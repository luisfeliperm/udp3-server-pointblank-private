namespace Core.managers.events
{
    public static class EventLoader
    {
        public static void LoadAll()
        {
            EventVisitSyncer.GenerateList();
            EventLoginSyncer.GenerateList();
            EventQuestSyncer.GenerateList();
            EventXmasSyncer.GenerateList();
        }
        public static void ReloadEvent(int index)
        {
            switch (index)
            {
                case 2:
                    EventVisitSyncer.ReGenList();
                    break;
                case 3:
                    EventLoginSyncer.ReGenList();
                    break;
                case 4:
                    EventQuestSyncer.ReGenList();
                    break;
                case 5:
                    EventXmasSyncer.ReGenList();
                    break;
            }
        }
    }
}