using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [Header("Prefabs reales de los coches (los mismos que en el menú)")]
    public GameObject[] carPrefabs;

    public Transform spawnPoint;

    void Start()
    {
        int selectedIndex = PlayerPrefs.GetInt("SelectedCar", 0);

        if (selectedIndex < 0 || selectedIndex >= carPrefabs.Length)
        {
            selectedIndex = 0;
        }

        GameObject car = Instantiate(
            carPrefabs[selectedIndex],
            spawnPoint.position,
            spawnPoint.rotation
        );

        // Muy importante: aquí NO tocamos el script Vehicle.
        // El asset ya se encarga de leer el input y mover el coche.
    }
}
