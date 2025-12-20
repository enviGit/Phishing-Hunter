namespace ph.Managers.Save {
    public interface IDataPersistence {
        void LoadData(GameData data);
        void SaveData(ref GameData data);
    }
}
