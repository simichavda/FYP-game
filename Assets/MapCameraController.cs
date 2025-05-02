using UnityEngine;
using UnityEngine.Android;
using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using System.Collections;

public class MapCameraController : MonoBehaviour
{

    public ArcGISLocationComponent cameraLocationComponent;
    public float DesiredAccuracyInMeters = 1.0f;
    public float UpdateDistanceInMeters = 1.0f;
    public float AltitudeOffset = 3000.0f; // Offset for camera's altitude in meters-

    public ArcGISPoint debugPosition = new ArcGISPoint(0, 0, 0, ArcGISSpatialReference.WGS84());

    private bool isLocationServiceRunning = false;
    private Coroutine locationUpdateCoroutine;
    public float LocationUpdateInterval = 1.0f; // Check for new location every second

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        if(!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            // Request permission to access location, watch callbacks for success or failure
            var callbacks = new PermissionCallbacks();
            Permission.RequestUserPermission(Permission.FineLocation, callbacks);

            callbacks.PermissionGranted += (permission) =>
            {
                Debug.Log("Permission granted: " + permission);
            };

            callbacks.PermissionDenied += (permission) =>
            {
                Debug.Log("Permission denied: " + permission);
            };
        }

        if(!Input.location.isEnabledByUser)
        {
            Debug.Log("Location services are not enabled by the user.");
            yield break;
        }

        Input.location.Start(DesiredAccuracyInMeters, UpdateDistanceInMeters);


        // Wait until service initializes
        int maxWait = 20; // 20 seconds timeout
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        // Service didn't initialize in time
        if (maxWait < 1)
        {
            Debug.LogError("Location Service timed out.");
            yield break;
        }
        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location.");
            yield break;
        }
        else
        {
            Debug.Log("Location Service Initialized. Status: " + Input.location.status);
            isLocationServiceRunning = (Input.location.status == LocationServiceStatus.Running);
            if (isLocationServiceRunning)
            {
                // Start polling for updates
                locationUpdateCoroutine = StartCoroutine(LocationUpdateCoroutine());

                // Get the initial location
                LocationInfo locationInfo = Input.location.lastData;
                UpdateCameraPosition(locationInfo);
                Debug.Log("Initial location: " + locationInfo.latitude + ", " + locationInfo.longitude + ", " + locationInfo.altitude);
            }
        }
    }

    IEnumerator LocationUpdateCoroutine()
    {
        while (isLocationServiceRunning)
        {
            // Check status again in case it stopped
            if (Input.location.status != LocationServiceStatus.Running)
            {
                Debug.LogWarning("Location service stopped or failed.");
                isLocationServiceRunning = false;
                yield break;
            }

            LocationInfo locationInfo = Input.location.lastData;
            UpdateCameraPosition(locationInfo);

            yield return new WaitForSeconds(LocationUpdateInterval);
        }
    }

    void UpdateCameraPosition(LocationInfo locationInfo)
    {
        ArcGISPoint geographicCoordinates = new ArcGISPoint(locationInfo.latitude, locationInfo.longitude, locationInfo.altitude + AltitudeOffset, ArcGISSpatialReference.WGS84());
        cameraLocationComponent.Position = geographicCoordinates;

        ArcGISRotation arcGISRotation = new ArcGISRotation(65, 28, 0);
        cameraLocationComponent.Rotation = arcGISRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(debugPosition.X != 0 && debugPosition.Y != 0)
        {
            ArcGISPoint geographicCoordinates = new ArcGISPoint(debugPosition.X, debugPosition.Y, debugPosition.Z + AltitudeOffset, ArcGISSpatialReference.WGS84());
            cameraLocationComponent.Position = geographicCoordinates;

            ArcGISRotation arcGISRotation = new ArcGISRotation(65, 28, 0);
            cameraLocationComponent.Rotation = arcGISRotation;
        }
    }
}
