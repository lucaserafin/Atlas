namespace Atlas.Api.Domain;

public abstract class Entity
{
    int _id;
    public virtual int Id
    {
        get
        {
            return _id;
        }
        protected set
        {
            _id = value;
        }
    }

    Guid _guid;

    public virtual Guid Guid
    {
        get
        {
            return _guid;
        }
        protected set
        {
            _guid = value;
        }
    }
}
