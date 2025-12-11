using UnityEngine;
using UnityEngine.SceneManagement;

public class CarSelectionSingle : MonoBehaviour
{
    [Header("Coches para seleccionar (tus prefabs reales)")]
    public GameObject[] carPrefabs;

    [Header("Punto donde se ve la vista previa")]
    public Transform previewPoint;

    [Header("Escala de la vista previa")]
    public float previewScale = 3f;   // súbelo a 5, 8, 10... hasta que te guste

    private int currentIndex = 0;
    private GameObject currentPreview;

    void Start()
    {
        UpdatePreview();
    }

    public void NextCar()
    {
        currentIndex++;
        if (currentIndex >= carPrefabs.Length)
            currentIndex = 0;

        UpdatePreview();
    }

    public void PrevCar()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = carPrefabs.Length - 1;

        UpdatePreview();
    }

   void UpdatePreview()
{
    if (currentPreview != null)
        Destroy(currentPreview);

    currentPreview = Instantiate(
        carPrefabs[currentIndex],
        previewPoint.position,
        previewPoint.rotation * Quaternion.Euler(15, 180f, 0)
    );

    Rigidbody rb = currentPreview.GetComponent<Rigidbody>();
    if (rb != null)
        rb.isKinematic = true;

    // Actualizar el objetivo de la cámara orbitante
    PreviewCameraOrbit orbit = Camera.main.GetComponent<PreviewCameraOrbit>();
    if (orbit != null)
        orbit.target = currentPreview.transform;
}


    public void PlayGame(string sceneName)
    {
        PlayerPrefs.SetInt("SelectedCar", currentIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene(sceneName);
    }
}
