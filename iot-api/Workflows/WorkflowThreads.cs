using System.Collections.Concurrent;

namespace iot_api.Workflows
{
    public static class WorkflowThreads
    {
        private static ConcurrentDictionary<string, WorkflowThreadStatus> _workflowThreads;

        public static void Add(string workflowId)
        {
            if (_workflowThreads == null)
                _workflowThreads = new ConcurrentDictionary<string, WorkflowThreadStatus>();

            _workflowThreads.TryAdd(workflowId, WorkflowThreadStatus.Running);
        }

        public static bool IsCancelled(string workflowId)
        {
            if (_workflowThreads != null && _workflowThreads.ContainsKey(workflowId))
                return _workflowThreads[workflowId] == WorkflowThreadStatus.Cancelled;

            return false;
        }

        public static void Cancel(string workflowId)
        {
            if (_workflowThreads == null)
                _workflowThreads = new ConcurrentDictionary<string, WorkflowThreadStatus>();

            if (_workflowThreads.ContainsKey(workflowId))
                _workflowThreads.TryUpdate(workflowId, WorkflowThreadStatus.Cancelled, WorkflowThreadStatus.Running);
            else
                _workflowThreads.TryAdd(workflowId, WorkflowThreadStatus.Cancelled);
        }

        public static void Remove(string workflowId)
        {
            if (_workflowThreads != null && _workflowThreads.ContainsKey(workflowId))
                _workflowThreads.TryRemove(workflowId, out _);
        }

        private enum WorkflowThreadStatus
        {
            Running,
            Cancelled
        }
    }
}