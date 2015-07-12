using System;
using System.Runtime.Serialization;

namespace Matcha.WebApi.Domain.DataAccess
{
    [Serializable]
    public class EntityNotFoundException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public EntityNotFoundException()
        {
        }

        public EntityNotFoundException(string message)
            : base(message)
        {
        }

        public EntityNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected EntityNotFoundException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        public static EntityNotFoundException From<T>(Guid id)
        {
            return new EntityNotFoundException(string.Format("Could not find {0} with Id {1}", typeof(T).Name, id));
        }
    }
}