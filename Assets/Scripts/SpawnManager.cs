using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] trafficCarPrefab;
    public GameObject busPrefab;
    public GameObject copCarPrefab;
    public GameObject copPrefab;
    public int trafficCarsLimit;
    public int busesLimit;
    public int copCarsLimit;
    public int copsLimit;

    int trafficCarsAmount;
    int busesAmount;
    int copCarsAmount;
    int copsAmount;
    int randInd;
    float zPos;
    Vector3 spawnPos;

    private void Start()
    {
        for (int i = 0; i < trafficCarsLimit; i++) // Cars
        {            
            Vector3 randSpawnPos = new Vector3(Rand(-6, 6), 0.75f, Rand(-1495, 2495));
            Instantiate(trafficCarPrefab[randInd], randSpawnPos, trafficCarPrefab[randInd].transform.rotation);
        }
        for (int i = 0; i < busesLimit; i++) // Buses
        {
            Vector3 randSpawnPos = new Vector3(6, 1.6f, Rand(-1495, 2495));
            Instantiate(busPrefab, randSpawnPos, busPrefab.transform.rotation);
        }
        for (int i = 0; i < copCarsLimit; i++) // CopCars
        {
            Vector3 randSpawnPos = new Vector3(10, 0.75f, Rand(-1495, 2495));
            Instantiate(copCarPrefab, randSpawnPos, copCarPrefab.transform.rotation);
        }
        for (int i = 0; i < copsLimit; i++) // Cops
        {
            Vector3 randSpawnPos = new Vector3(8.5f, 1f, Rand(-1495, 2495));
            Instantiate(copPrefab, randSpawnPos, copPrefab.transform.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        trafficCarsAmount = GameObject.FindGameObjectsWithTag("Traffic Car").Length;
        busesAmount = GameObject.FindGameObjectsWithTag("Bus").Length;
        copCarsAmount = GameObject.FindGameObjectsWithTag("CopCar").Length;
        copsAmount = GameObject.FindGameObjectsWithTag("Cop").Length;
        randInd = Random.Range(0, trafficCarPrefab.Length);

        if (Input.GetAxis("Vertical") > 0)
        {
            zPos = 120;
        }
        else
            zPos = -50;

        if (trafficCarsAmount < trafficCarsLimit)
        {
            SpawnTrafficCar();
        }

        if (busesAmount < busesLimit)
        {
            SpawnBus();
        }

        if (copCarsAmount < copCarsLimit)
        {
            SpawnCopCar();
        }

        if (copsAmount < copsLimit)
        {
            SpawnCop();
        }
    }

    float Rand(float min, float max)
    {
        return Random.Range(min, max);        
    }

    void SpawnTrafficCar()
    {
        spawnPos = new Vector3(Random.Range(-6, 6), 0.75f, zPos);
        Instantiate(trafficCarPrefab[randInd], spawnPos, trafficCarPrefab[randInd].transform.rotation);
    }

    void SpawnBus()
    {
        spawnPos = new Vector3(6, 1.6f, zPos);
        Instantiate(busPrefab, spawnPos, busPrefab.transform.rotation);
    }

    void SpawnCopCar()
    {
        spawnPos = new Vector3(10, 0.75f, zPos);
        Instantiate(copCarPrefab, spawnPos, copCarPrefab.transform.rotation);
    }

    void SpawnCop()
    {
        spawnPos = new Vector3(8.5f, 1f, zPos);
        Instantiate(copPrefab, spawnPos, copPrefab.transform.rotation);
    }
}
