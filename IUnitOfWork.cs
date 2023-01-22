using System;

namespace Webolar.Framework
{
    public interface IUnitOfWork<TEntity> : IDisposable where TEntity : class
    {
    }
}
