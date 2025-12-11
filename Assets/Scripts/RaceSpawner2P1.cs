using UnityEngine;

public class RaceSpawner2P : MonoBehaviour
{
    [Header("Prefabs reales de los coches (con físicas y control)")]
    public GameObject[] carPrefabs;

    public Transform spawnPointP1;
    public Transform spawnPointP2;

    void Start()
    {
        int indexP1 = PlayerPrefs.GetInt("SelectedCarP1", 0);
        int indexP2 = PlayerPrefs.GetInt("SelectedCarP2", 1);

        // Instanciar coche jugador 1
        GameObject car1 = Instantiate(
            carPrefabs[indexP1],
            spawnPointP1.position,
            spawnPointP1.rotation);

        // Instanciar coche jugador 2
        GameObject car2 = Instantiate(
            carPrefabs[indexP2],
            spawnPointP2.position,
            spawnPointP2.rotation);

        // De momento NO tocamos los scripts de movimiento aquí
        // Cada coche se mueve con el script que ya tiene.
    }
}
