using System.Reflection;

namespace IndexedDB.Blazor.Extensions
{
    internal static class TaskExtensions
    {
        internal static async Task<object?> InvokeAsyncWithResult(this MethodInfo @this, object obj, params object[] parameters)
        {
            if (@this.Invoke(obj, parameters) is not Task task)
                return null;
            await task.ConfigureAwait(false);
            var resultProperty = task.GetType().GetProperty("Result");
            if(resultProperty == null) 
                return null;
            return resultProperty.GetValue(task);
        }

        internal static async Task InvokeAsync(this MethodInfo @this, object obj, params object[] parameters)
        {
            if (@this.Invoke(obj, parameters) is not Task task)
                throw new Exception("Instance not created");
            await task.ConfigureAwait(false);
        }
    }
}
