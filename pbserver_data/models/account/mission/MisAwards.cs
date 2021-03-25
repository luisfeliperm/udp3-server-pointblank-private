namespace Core.models.account.mission
{
    public class MisAwards
    {
        public int _id, _blueOrder, _exp, _gp;
        public MisAwards(int id, int blueOrder, int exp, int gp)
        {
            _id = id;
            _blueOrder = blueOrder;
            _exp = exp;
            _gp = gp;
        }
    }
}