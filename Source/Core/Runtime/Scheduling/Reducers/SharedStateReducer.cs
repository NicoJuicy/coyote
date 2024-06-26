﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Coyote.Runtime
{
    /// <summary>
    /// A reducer that analyzes shared state accesses to reduce the set of operations
    /// to be scheduled at each scheduling step.
    /// </summary>
    internal sealed class SharedStateReducer : IScheduleReducer
    {
        /// <summary>
        /// Set of values corresponding to shared state that has been accessed
        /// by 'READ' operations across all iterations.
        /// </summary>
        private readonly HashSet<string> ReadAccesses;

        /// <summary>
        /// Set of values corresponding to shared state that has been accessed
        /// by 'WRITE' operations across all iterations.
        /// </summary>
        private readonly HashSet<string> WriteAccesses;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedStateReducer"/> class.
        /// </summary>
        internal SharedStateReducer()
        {
            this.ReadAccesses = new HashSet<string>();
            this.WriteAccesses = new HashSet<string>();
        }

        /// <inheritdoc/>
        public void InitializeNextIteration(uint iteration)
        {
        }

        /// <inheritdoc/>
        public IEnumerable<ControlledOperation> ReduceOperations(IEnumerable<ControlledOperation> ops, ControlledOperation current)
        {
            // Find all operations that are not invoking a user-defined scheduling decision.
            var noReadOrWriteSchedulingOps = ops.Where(
                op => !SchedulingPoint.IsReadOrWrite(op.LastSchedulingPoint));
            if (noReadOrWriteSchedulingOps.Any())
            {
                // One or more operations exist that are not invoking a user-defined
                // scheduling decision, so return them.
                return noReadOrWriteSchedulingOps;
            }
            else
            {
                // Split the operations that are accessing shared state into a 'READ' and 'WRITE' group.
                var readAccessOps = ops.Where(op => op.LastSchedulingPoint is SchedulingPointType.Read);
                var writeAccessOps = ops.Where(op => op.LastSchedulingPoint is SchedulingPointType.Write);

                // Update the known 'READ' and 'WRITE' access sets so far.
                this.ReadAccesses.UnionWith(readAccessOps.Select(op => op.LastAccessedSharedState));
                this.WriteAccesses.UnionWith(writeAccessOps.Select(op => op.LastAccessedSharedState));

                // Find if any operations are explicitly interleaving, and if yes do not perform any reduction.
                if (!ops.Any(op => op.LastSchedulingPoint is SchedulingPointType.Interleave ||
                    op.LastSchedulingPoint is SchedulingPointType.Yield))
                {
                    // Find if there are any read/write-only accesses. Note that this is just an approximation based on
                    // current knowledge. An access that is considered read/write-only might not be considered anymore
                    // in later steps or iterations once the known 'READ' and 'WRITE' access sets have been updated.
                    var readOnlyAccessOps = readAccessOps.Where(op => !this.WriteAccesses.Any(
                        state => op.LastAccessedSharedStateComparer?.Equals(op.LastAccessedSharedState, state) ??
                        op.LastAccessedSharedState == state));

                    if (readOnlyAccessOps.Any())
                    {
                        // Return all read-only access operations.
                        return readOnlyAccessOps;
                    }
                }
            }

            return ops;
        }
    }
}
