using UnityEngine;
using TMPro;
using System.Collections;

public class OwnProfile : MonoBehaviour {
    [SerializeField] TextMeshProUGUI date;
    [SerializeField] TextMeshProUGUI time;
    [SerializeField] TextMeshProUGUI timeAMPM;

    private void Start() {
        StartCoroutine(UpdateTimeCoroutine());
    }
    private IEnumerator UpdateTimeCoroutine() {
        while (true) {
            UpdateTime();
            yield return new WaitForSeconds(60f);
        }
    }
    private void UpdateTime() {
        System.DateTime currentTime = System.DateTime.Now;
        time.text = currentTime.ToString("hh:mm");
        timeAMPM.text = currentTime.ToString("tt");
        date.text = currentTime.ToString("dd/MM/yyyy");
    }
}
