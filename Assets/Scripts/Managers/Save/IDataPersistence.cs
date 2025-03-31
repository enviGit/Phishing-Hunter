using ph.Managers.Save;

namespace ph {
    public interface IDataPersistence {
        void LoadData(GameData data);
        void SaveData(ref GameData data);
    }
}
