using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace HighwayRacers
{
	/// <summary>
	/// Singleton class containing math functions.
	/// </summary>
    public class Highway : MonoBehaviour
    {
        public const int NUM_LANES = 4;
        public const float LANE_SPACING = 1.9f;
        public const float MID_RADIUS = 31.46f;
        public const float CURVE_LANE0_RADIUS = MID_RADIUS - LANE_SPACING * (NUM_LANES - 1) / 2f;
        public const float MIN_HIGHWAY_LANE0_LENGTH = CURVE_LANE0_RADIUS * 4;
        public const float MIN_DIST_BETWEEN_CARS = .7f;

        [Header("Prefabs")]
        public GameObject carPrefab;
        public GameObject straightPiecePrefab;
        public GameObject curvePiecePrefab;

        public Mesh EntityCarMesh;
        public Material EntityCarMaterial;

        public static Highway instance { get; private set; }

        private HighwayPiece[] pieces = new HighwayPiece[8];

		/// <summary>
		/// Use this to access geometry from jobs
		/// </summary>
        public DotsHighway DotsHighway = new DotsHighway();

        public void CreateHighway(float lane0Length)
        {
            if (lane0Length < MIN_HIGHWAY_LANE0_LENGTH)
            {
                Debug.LogError("Highway length must be longer than " + MIN_HIGHWAY_LANE0_LENGTH);
                return;
            }

			int tempNumCars = NumCars;
			if (lane0Length < DotsHighway.Lane0Length)
				ClearCars();

            float straightPieceLength = (lane0Length - CURVE_LANE0_RADIUS * 4) / 4;

            Vector3 pos = Vector3.zero;
            float rot = 0;

            for (int i = 0; i < 8; i++)
            {
                if (i % 2 == 0)
                {
                    // straight piece
                    if (pieces[i] == null)
                    {
                        pieces[i] = Instantiate(straightPiecePrefab, transform).GetComponent<StraightPiece>();
                    }
                    StraightPiece straightPiece = pieces[i] as StraightPiece;
                    straightPiece.SetStartPosition(pos);
                    straightPiece.startRotation = rot;
                    straightPiece.SetLength(straightPieceLength);

                    pos += straightPiece.startRotationQ * new Vector3(0, 0, straightPieceLength);
                }
                else
                {
                    // curve piece
                    if (pieces[i] == null)
                    {
                        pieces[i] = Instantiate(curvePiecePrefab, transform).GetComponent<CurvePiece>();
                    }
                    CurvePiece curvePiece = pieces[i] as CurvePiece;
                    curvePiece.SetStartPosition(pos);
                    curvePiece.startRotation = rot;

                    pos += curvePiece.startRotationQ * new Vector3(MID_RADIUS, 0, MID_RADIUS);
                    rot = Mathf.PI / 2 * (i / 2 + 1);
                }
            }
            DotsHighway.Create(pieces);
			SetNumCars(tempNumCars);
        }

		public int NumCars { get; private set; }

		public void SetNumCars(int numCars)
        {
            DotsHighway.SetNumCars(numCars);
            NumCars = numCars;
        }

        public void ClearCars()
        {
            DotsHighway.SetNumCars(0);
            NumCars = 0;
        }

		public Car GetCarAtScreenPosition(Vector3 screenPosition, float radius)
        {
/*
			foreach (Car car in cars) {
				Vector3 carScreenPos = Camera.main.WorldToScreenPoint(car.transform.position);
				carScreenPos.z = screenPosition.z;

				if (Vector3.Distance (screenPosition, carScreenPos) <= radius) {
					return car;
				}

			}
*/
			return null;

		}

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        private void Start()
        {
			CreateHighway(250);
        }

        private void Update()
        {
        }

        private void OnDestroy()
        {
            ClearCars();
            if (instance == this)
            {
                DotsHighway.Dispose();
                instance = null;
            }
        }
    }
}
