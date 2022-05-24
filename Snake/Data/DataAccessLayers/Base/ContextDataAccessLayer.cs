namespace Snake.Data.DataAccessLayers.Base
{
    public abstract class ContextDataAccessLayer
    {
        internal SnakeContext _context;

        public ContextDataAccessLayer(SnakeContext context)
        {
            _context = context;
        }
    }
}
