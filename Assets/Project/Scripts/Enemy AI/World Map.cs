using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AccrossTheEvergate
{
    public class WorldMap : MonoBehaviour
    {
        [SerializeField] private Transform playerObj;
        [SerializeField] private Canvas mapCanvas;
        [SerializeField] private RectTransform playerIconTransform;
        [SerializeField] private RectTransform mapRectTransform;

        //[SerializeField] float mapScaleFactorX = 1.5f;
        //[SerializeField] float mapScaleFactorY = 1.5f;
        [SerializeField] private GameObject[] areaNames;

        private bool mapIsOpen;
        private InputManager _inputManager;
        private int areaIndex;

        private void Awake()
        {
            //ServiceLocator.Instance.RegisterService<WorldMap>(this);
        }

        private void Start()
        {
            _inputManager = ServiceLocator.Instance.GetService<InputManager>();
            mapCanvas.enabled = false;
            mapIsOpen = false;
            areaIndex = 1;
        }

        // Update is called once per frame
        private void Update()
        {
            //if (_inputManager.isMapOpen)
            ToggleMap();

            if (mapIsOpen)
            {
                Vector3 playerPos = PlayerPositionOnMap(playerObj.position);
                playerIconTransform.anchoredPosition = playerPos;
                //Debug.Log(playerPos);
            }
        }

        private Vector2 PlayerPositionOnMap(Vector3 player)
        {
            //terrain dimensions
            float terrainWidth = 1850.88f;
            float terrainHeight = 1060f;

            //convert
            float normalizedX = player.x / terrainWidth;
            float normalizedZ = player.z / terrainHeight;

            // Apply scaling factor to normalized position
            float scalingFactor = 1.5f; // Adjust this value to increase or decrease the distance between villages
            normalizedX *= scalingFactor;
            normalizedZ *= scalingFactor;

            // Ensure normalized values stay within bounds [0, 1]
            normalizedX = Mathf.Clamp01(normalizedX);
            normalizedZ = Mathf.Clamp01(normalizedZ);

            //to map coord
            float mapWidth = mapRectTransform.rect.width;
            float mapHeight = mapRectTransform.rect.height;

            //scale
            float mapX = normalizedX * mapWidth;
            float mapY = normalizedZ * mapHeight;

            return new Vector2(mapX, mapY);
        }

        public void ToggleMap()
        {
            mapCanvas.enabled = !mapCanvas.enabled;
            mapIsOpen = mapCanvas.enabled;

            if (mapIsOpen)
            {
                playerIconTransform.anchoredPosition = PlayerPositionOnMap(playerObj.position);
            }

            //_inputManager.isMapOpen = false;
        }

        public void OpenArea(int areaId)
        {
            if (areaId >= 0 && areaId < areaNames.Length)
            {
                areaNames[areaId].gameObject.SetActive(true);
            }
        }

        public void PlaceVillageOnMap(GameObject village, Vector3 position)
        {
            float scalingFactor = 1.5f; // Adjust this value to increase or decrease the distance between villages

            // Apply scaling factor to position
            position.x *= scalingFactor;
            position.z *= scalingFactor;

            // Ensure position stays within map bounds
            position.x = Mathf.Clamp(position.x, 0, mapRectTransform.rect.width);
            position.z = Mathf.Clamp(position.z, 0, mapRectTransform.rect.height);

            // Place village at new position
            village.transform.position = position;
        }
    }
}