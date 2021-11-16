namespace AsyncArchitecture.Events.Models
{
    public class CudEvent<TEntity> : Event
    {
        public TEntity Entity { get; set; }

        public CudEventType CudEventType { get; set; }
    }
}