using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class Game : MonoBehaviour
    {
        [SerializeField]
        private float gridScale;

        [SerializeField]
        private Vector3 size;

        [SerializeField]
        private GameObject foodPrefab;
        
        [SerializeField]
        private int foodNum;

        [SerializeField]
        private Snake snake;

        private int cols;
        private int rows;
        
        private List<Food> foods = new List<Food>();

        public Vector3 Size
        {
            get { return size; }
            set { size = value; }
        }

        public float GridScale
        {
            get { return gridScale; }
            set { gridScale = value; }
        }

        private void Start()
        {
            cols = (int) (size.x * 2 / gridScale);
            rows = (int) (size.z * 2 / gridScale);

            CreateInitialFood();
        }

        private void CreateInitialFood()
        {
            for (int i = 0; i < foodNum; i++)
            {
                CreateFood();
            }
        }


        private void PickLocation()
        {
            foreach (var food in foods)
            {
                if (food.transform.position == snake.HeadPosition())
                {
                    EatFood(food);
                    return;
                }
            }            
        }

        private void EatFood(Food food)
        {
            foods.Remove(food);
            CreateFood();
            snake.Grow(food);
        }

        void Update()
        {
            PickLocation();
        }


        private void CreateFood()
        {
            GameObject foodPiece = Instantiate(foodPrefab);

            float x = Random.Range(transform.position.x - size.x, transform.position.x + size.x);
            float z = Random.Range(transform.position.z - size.z, transform.position.z + size.z);

            var halfScale = gridScale / 2;
            Vector3 pos = new Vector3(x - x % halfScale, 0, z - z % halfScale);
            
            foodPiece.transform.position = pos;
            
            foods.Add(foodPiece.GetComponent<Food>());
        }

        public void Win()
        {
            Debug.Log("Maximum Score!");
        }

        public void Lost()
        {
            Debug.Log("You loose!");
            snake.Reset();

            foreach (var food in foods)
            {
                Destroy(food.gameObject);
            }
            
            foods.Clear();
            
            CreateInitialFood();
        }
        
        
    }
}