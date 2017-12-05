/*
 * Author:      #AUTHORNAME#
 * CreateTime:  #CREATETIME#
 * Description:
 * 
*/
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EZhex1991
{
    public class PlayerController : MonoBehaviour
    {
        public GameObject element;
        public GameObject virtualElement;
        public bool canMove = true;
        public float speed = 1;
        public LayerMask moveBlockLayer;
        public LayerMask growBlockLayer;

        public int propCount_Brick;
        public event Action onStuck;
        public event Action<int> onBrickCountChanged;

        private Camera mainCamera;
        private GameObject illusion;
        private bool leftAvailable = true;
        private bool rightAvailable = true;
        private bool moving;
        private int moveToX;

        void Start()
        {
            mainCamera = Camera.main;
            illusion = Instantiate(virtualElement);
            illusion.SetActive(false);
        }

        void Update()
        {
            if (!leftAvailable && !rightAvailable)
            {
                if (onStuck != null) onStuck();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            if (canMove)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    leftAvailable = Move(-1);
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    rightAvailable = Move(1);
                }
            }
            else
            {
                moving = true;
                moveToX = Mathf.RoundToInt(transform.position.x);
            }

            illusion.SetActive(false);
            if (moving)
            {
                float distance = moveToX - transform.position.x;
                if (Mathf.Abs(distance) > 1e-5)
                {
                    float x = Mathf.MoveTowards(transform.position.x, moveToX, speed * Time.deltaTime);
                    transform.position = new Vector2(x, transform.position.y);
                }
                else
                {
                    transform.position = new Vector2(Mathf.RoundToInt(transform.position.x), transform.position.y);
                    moving = false;
                }
            }
            else
            {
                Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                if (propCount_Brick > 0 && CanGrow(ref mousePosition))
                {
                    illusion.transform.position = mousePosition;
                    illusion.SetActive(true);
                    if (Input.GetMouseButtonDown(0))
                    {
                        Grow(mousePosition);
                    }
                }
            }
        }

        bool Move(int movement)
        {
            bool onGround = false;
            Vector2 start, end;
            RaycastHit2D hit;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                start = child.position;
                end = start + new Vector2(movement, 0);
                hit = Physics2D.Linecast(start, end, moveBlockLayer);
                if (hit)
                {
                    return false;
                }
                end = start + new Vector2(0, -1);
                hit = Physics2D.Linecast(start, end, moveBlockLayer);
                if (hit)
                {
                    onGround = true;
                }
            }
            if (!onGround)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    start = child.position;
                    end = start + new Vector2(movement, -1);
                    hit = Physics2D.Linecast(start, end, moveBlockLayer);
                    if (hit)
                    {
                        return false;
                    }
                }
            }
            moving = true;
            moveToX = Mathf.RoundToInt(transform.position.x + movement * 0.6f);
            return true;
        }

        bool CanGrow(ref Vector2 position)
        {
            position = Snap2Int(position);
            if (Physics2D.OverlapCircle(position, 0.3f, growBlockLayer))
                return false;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (((Vector2)transform.GetChild(i).position - position).magnitude < 1.1)
                {
                    return true;
                }
            }
            return false;
        }
        void Grow(Vector2 position)
        {
            GameObject go = Instantiate(element, position, Quaternion.identity) as GameObject;
            go.transform.SetParent(transform);
            go.transform.localPosition = Snap2Int(go.transform.localPosition);
            propCount_Brick--;
            if (onBrickCountChanged != null) onBrickCountChanged(propCount_Brick);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Prop_Brick_1")
            {
                propCount_Brick += 1;
                other.gameObject.SetActive(false);
                if (onBrickCountChanged != null) onBrickCountChanged(propCount_Brick);
            }
            if (other.tag == "Prop_Brick_4")
            {
                propCount_Brick += 4;
                other.gameObject.SetActive(false);
                if (onBrickCountChanged != null) onBrickCountChanged(propCount_Brick);
            }
        }

        Vector2 Snap2Int(Vector2 vect)
        {
            return new Vector2(Mathf.RoundToInt(vect.x), Mathf.RoundToInt(vect.y));
        }
    }
}