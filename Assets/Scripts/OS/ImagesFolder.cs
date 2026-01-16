using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace ph.OS {
    public class ImagesFolder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        private bool isSelected;
        public GameObject selected;
        public GameObject actualFolder;
        public Animator selectedAnim;
        private bool folderIsRunning = false;
        public GameObject folderInTaskbar;
        public Animator folderAnim;
        public GameObject image;
        public GameObject panel;
        public Transform newImageParent;
        public Transform newPanelParent;
        private RawImage chosenImage;
        public int imageNumber = 0;
        public int panelNumber = 0;
        public Animator newImageAnim;
        [SerializeField] TextMeshProUGUI error;
        public bool inSelectionState = false;
        public RawImage wallpaper;
        void Start() {

        }

        void Update() {
            imageNumber = GameObject.FindGameObjectsWithTag("NewImages").Length + 1;
            panelNumber = imageNumber;
            if (isSelected) {
                selected.SetActive(true);
                if (Input.GetKeyDown(KeyCode.Mouse0)) {
                    folderIsRunning = true;
                    actualFolder.SetActive(true);
                    selectedAnim.SetTrigger("Clicked");
                }
            }
            else {
                selected.SetActive(false);
            }
            if (folderIsRunning) {
                folderInTaskbar.SetActive(true);
            }
            else {
                folderInTaskbar.SetActive(false);
            }
            GameObject[] scaleSliders = GameObject.FindGameObjectsWithTag("ScaleSliders");
            foreach (GameObject scaleSlider in scaleSliders) {
                scaleSlider.transform.parent.transform.Find("ImageMask").transform.Find("Image").GetComponent<RectTransform>().localScale = new Vector2(scaleSlider.GetComponent<Slider>().value, scaleSlider.GetComponent<Slider>().value);
            }
        }
        public void OnPointerEnter(PointerEventData pointerEventData) {
            isSelected = true;
        }

        public void OnPointerExit(PointerEventData pointerEventData) {
            isSelected = false;
        }
        public void AddImage() {
            if (imageNumber == 1) {
                inSelectionState = false;
            }
            if (inSelectionState == true && imageNumber > 0) {
                error.text = "Please quit the selection state.";
            }
            else {
                error.text = "";
                OpenFileBrowser();
            }
        }

        public void OpenFileBrowser() {
            var bp = new BrowserProperties();
            bp.filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            bp.filterIndex = 0;

            new FileBrowser().OpenFileBrowser(bp, path => {
                StartCoroutine(LoadImage(path));
            });
        }

        IEnumerator LoadImage(string path) {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path)) {
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError) {
                    Debug.Log(uwr.error);
                }
                else {
                    var uwrTexture = DownloadHandlerTexture.GetContent(uwr);
                    GameObject newImage = Instantiate(image);
                    newImage.gameObject.tag = "NewImages";
                    newImage.transform.SetParent(newImageParent);
                    newImage.transform.localScale = new Vector2(1, 1);
                    newImage.gameObject.SetActive(true);
                    newImage.gameObject.transform.Find("Image").gameObject.SetActive(true);
                    newImage.gameObject.transform.Find("Image").transform.Find("Title").GetComponent<TextMeshProUGUI>().text = "Image - " + imageNumber.ToString();
                    newImageAnim = newImage.gameObject.GetComponent<Animator>();
                    GameObject newPanel = Instantiate(panel);
                    newPanel.gameObject.tag = "NewPanels";
                    newPanel.gameObject.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = "Image - " + panelNumber.ToString();
                    newPanel.transform.SetParent(newPanelParent);
                    newPanel.transform.localScale = new Vector2(1, 1);
                    newPanel.transform.position = new Vector2(1915, -141.27f);
                    newPanel.gameObject.SetActive(true);
                    newImage.gameObject.transform.Find("Image").GetComponent<Button>().onClick.AddListener(() => newPanel.transform.position = panel.transform.position);
                    newPanel.transform.Find("ImageMask").transform.Find("Image").GetComponent<RawImage>().texture = uwrTexture;
                    newPanel.transform.Find("ImageMask").transform.Find("Image").GetComponent<RectTransform>().sizeDelta = new Vector2(uwrTexture.width, uwrTexture.height);
                    newPanel.transform.Find("SetWallpaper").GetComponent<Button>().onClick.AddListener(() => wallpaper.texture = newPanel.transform.Find("ImageMask").transform.Find("Image").GetComponent<RawImage>().texture);
                }
            }
        }
        public void SelectState() {
            GameObject[] newImages = GameObject.FindGameObjectsWithTag("NewImages");
            if (imageNumber > 1) {
                error.text = "";
                foreach (GameObject newOne in newImages) {
                    if (newOne.GetComponent<Animator>().GetBool("inSelectState") == true) {
                        inSelectionState = false;
                        newOne.GetComponent<Animator>().SetBool("inSelectState", false);
                    }
                    else {
                        inSelectionState = true;
                        newOne.GetComponent<Animator>().SetBool("inSelectState", true);
                    }
                }
            }
            else {
                error.text = "Please Create an image.";
            }
        }
        public void SelectAll() {
            GameObject[] newImages = GameObject.FindGameObjectsWithTag("NewImages");
            foreach (GameObject newOne in newImages) {
                if (newOne.GetComponent<Animator>().GetBool("inSelectState") == true) {
                    newOne.transform.Find("Image").transform.Find("Toggle").GetComponent<Toggle>().isOn = true;
                    error.text = "";
                }
                else {
                    error.text = "Please enter the selection state.";
                }
            }
            if (imageNumber == 1) {
                error.text = "Please Create an image.";
            }
        }
        public void DeleteImage() {
            GameObject[] newImages = GameObject.FindGameObjectsWithTag("NewImages");
            GameObject[] newPanels = GameObject.FindGameObjectsWithTag("NewPanels");
            if (inSelectionState == true) {
                error.text = "";
                foreach (GameObject newOneImage in newImages) {
                    foreach (GameObject newOnePanel in newPanels) {
                        if (newOneImage.transform.Find("Image").transform.Find("Toggle").GetComponent<Toggle>().isOn) {
                            if (newOnePanel.gameObject.transform.Find("Title").GetComponent<TextMeshProUGUI>().text == newOneImage.gameObject.transform.Find("Image").transform.Find("Title").GetComponent<TextMeshProUGUI>().text) {
                                Destroy(newOnePanel);
                                Destroy(newOneImage);
                            }
                        }
                    }
                }
            }
            if (inSelectionState == false && imageNumber > 1) {
                error.text = "Please Select an image.";
            }
            if (inSelectionState == false && imageNumber == 1) {
                error.text = "Please Create an image.";
            }
        }
        public void Exit() {
            folderIsRunning = false;
        }
    }
}
