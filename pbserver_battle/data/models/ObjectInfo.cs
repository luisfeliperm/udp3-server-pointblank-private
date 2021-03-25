using Battle.data.xml;
using System;

namespace Battle.data.models
{
    public class ObjectInfo
    {
        public int _id, _life = 100, DestroyState;
        public AnimModel _anim;
        public DateTime _useDate;
        public ObjModel _model;
        public ObjectInfo()
        {
        }
        public ObjectInfo(int id)
        {
            _id = id;
        }
    }
}