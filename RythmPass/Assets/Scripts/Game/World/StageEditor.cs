using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Animations;

namespace Dev
{
    public class StageEditor : MonoBehaviour
    {
        enum EditorScope
        {
            Tile,
            StaticObject,
            WorldCharacter,
        }

        public GameField GameField => m_GameField;

        [SerializeField] GameObject m_CameraPrefab;
        [SerializeField] WorldCamera m_WorldCamera;
        [SerializeField] TileObject m_DefaultTileObject;
        [SerializeField] GameField m_GameField;
        [SerializeField] GameObject m_WorldObjects;
        [SerializeField] GameObject m_QuadGo;
        [SerializeField] GameObject m_TileGroupGo;
        [SerializeField] LookAtConstraint m_LookAtConstraint;

        TileObject m_SelectedTileObject;
        StaticObject m_SelectedStaticObject;
        WorldCharacter m_SelectedWorldCharacter;
        EditorScope m_Scope;
        int m_Height;
        int m_Width;
        readonly List<TileObject> m_Tiles = new();

        public void LoadGameField(GameField gameField)
        {
            if (null != m_GameField)
            {
                Destroy(m_GameField.gameObject);
                m_Tiles.Clear();
            }
            m_GameField = Instantiate(gameField);
            m_WorldObjects = m_GameField.transform.Find("WorldObject").gameObject;
            m_CameraPrefab = m_GameField.WorldCamera.transform.parent.gameObject;
            m_WorldCamera = m_GameField.WorldCamera;
            m_LookAtConstraint = m_WorldCamera.GetComponentInChildren<LookAtConstraint>();
            m_LookAtConstraint.constraintActive = false;
            m_QuadGo = m_GameField.GetComponentInChildren<MeshFilter>().gameObject;
            var tileGroup = m_GameField.GetComponentInChildren<TileGroup>();
            m_TileGroupGo = tileGroup.gameObject;
            m_Tiles.AddRange(tileGroup.Tiles);
            m_Width = m_GameField.CellSize.x;
            m_Height = m_GameField.CellSize.y;
            Refresh();
        }

        public void Init()
        {
            m_Scope = EditorScope.Tile;
            m_SelectedTileObject = m_DefaultTileObject;
            m_SelectedStaticObject = null;
            m_SelectedWorldCharacter = null;
            m_LookAtConstraint.constraintActive = false;
        }

        public void SetSelectTileObject(TileObject tile)
        {
            m_SelectedTileObject = tile;
            m_Scope = EditorScope.Tile;
        }

        public void SetSelectStaticObject(StaticObject obj)
        {
            m_SelectedStaticObject = obj;
            m_Scope = EditorScope.StaticObject;
        }

        public void SetSelectWorldCharacter(WorldCharacter obj)
        {
            m_SelectedWorldCharacter = obj;
            m_Scope = EditorScope.WorldCharacter;
        }

        public void SetSize(int width, int height)
        {
            int newCount = width * height;
            int prevCount = m_Tiles.Count;
            if (newCount > prevCount)
            {
                int needCount = newCount - prevCount;
                FillDeficiencyTile(needCount);
            }
            else if (newCount < prevCount)
            {
                int excessCount = prevCount - newCount;
                RemoveExcessTile(excessCount);
            }
            m_Width = width;
            m_Height = height;
            Refresh();
        }

        public TileObject GetTile(int x, int y)
        {
            if (x < 0 || x >= m_Width || y < 0 || y >= m_Height)
            {
                return null;
            }
            int index = m_Width * y + x;
            if (!m_Tiles.IsInIndex(index))
            {
                return null;
            }
            return m_Tiles[index];
        }

        public void SetTile(int x, int y, TileObject clone)
        {
            if (x < 0 || x >= m_Width || y < 0 || y >= m_Height)
            {
                return;
            }
            int index = m_Width * y + x;
            if (!m_Tiles.IsInIndex(index))
            {
                return;
            }
            var tile = m_Tiles[index];
            var siblingIndex = tile.transform.GetSiblingIndex();
            DestroyImmediate(m_Tiles[index].gameObject);
            tile = Instantiate(clone);
            tile.name = $"Tile{x}{y}";
            tile.transform.SetParent(m_TileGroupGo.transform);
            tile.transform.SetSiblingIndex(siblingIndex);
            m_Tiles[index] = tile;
            Refresh();
        }

        public void SetStaticObject(int x, int y, StaticObject clone)
        {
            if (x < 0 || x >= m_Width || y < 0 || y >= m_Height)
            {
                return;
            }
            int index = m_Width * y + x;
            if (!m_Tiles.IsInIndex(index))
            {
                return;
            }
            var tile = m_Tiles[index];
            if (m_Tiles[index].StaticObject != null)
            {
                DestroyImmediate(m_Tiles[index].StaticObject.gameObject);
            }
            var obj = Instantiate(clone);
            obj.transform.SetParent(m_WorldObjects.transform);
            obj.SetIndex(tile.Index);
            obj.FitPosition();
            tile.BindStaticObject(obj);
            Refresh();
        }

        public void SetWorldCharacter(int x, int y, WorldCharacter clone)
        {
            if (x < 0 || x >= m_Width || y < 0 || y >= m_Height)
            {
                return;
            }
            int index = m_Width * y + x;
            if (!m_Tiles.IsInIndex(index))
            {
                return;
            }
            var tile = m_Tiles[index];
            foreach (var ch in m_Tiles[index].WorldCharacters)
            {
                Destroy(ch.gameObject);
            }
            m_Tiles[index].WorldCharacters.Clear();
            var obj = Instantiate(clone);
            obj.transform.SetParent(m_WorldObjects.transform);
            obj.SetIndex(tile.Index);
            obj.FitPosition();
            tile.OccupyTile(obj);
            Refresh();
        }

        void FillDeficiencyTile(int needCount)
        {
            for (int i = 0; i < needCount; ++i)
            {
                var tile = Instantiate(m_DefaultTileObject);
                m_Tiles.Add(tile);
            }
        }

        void RemoveExcessTile(int excessCount)
        {
            for (int i = 0; i <excessCount; ++i)
            {
                int lastIndex = m_Tiles.Count - 1;
                DestroyImmediate(m_Tiles[lastIndex].gameObject);
                m_Tiles.RemoveAt(lastIndex);
            }
        }

        void Refresh()
        {
            for (int i = 0; i < m_Tiles.Count; ++i)
            {
                int indexX = i % m_Width;
                int indexY = i / m_Width;
                var tile = m_Tiles[i];
                tile.gameObject.SetParent(m_TileGroupGo.gameObject);
                tile.SetIndex(new Vector2Int(indexX, indexY));
            }
            m_GameField.CellSize = new Vector2Int(m_Width, m_Height);
            m_GameField.Validate();
        }

        // ========================================================================

        void Update()
        {
            if (Input.GetMouseButton(0) && null != m_WorldCamera && !EventSystem.current.IsPointerOverGameObject())
            {
                var ray = m_WorldCamera.GetRay(Input.mousePosition);
                if (!GameField.ZeroPlane.Raycast(ray, out var factor))
                {
                    return;
                }
                var hitPosition = ray.GetPoint(factor);
                var hitIndex = new Vector2Int(Mathf.RoundToInt(hitPosition.x), Mathf.RoundToInt(hitPosition.z));
                switch (m_Scope)
                {
                    case EditorScope.WorldCharacter:
                        SetWorldCharacter(hitIndex.x, hitIndex.y, m_SelectedWorldCharacter);
                        break;
                    case EditorScope.Tile:
                        SetTile(hitIndex.x, hitIndex.y, m_SelectedTileObject);
                        break;
                    case EditorScope.StaticObject:
                        SetStaticObject(hitIndex.x, hitIndex.y, m_SelectedStaticObject);
                        break;
                }
            }

            if (Input.GetKey(KeyCode.W))
            {
                MoveCamera(Vector3.forward * 3f);
            }
            if (Input.GetKey(KeyCode.A))
            {
                MoveCamera(Vector3.left * 3f);
            }
            if (Input.GetKey(KeyCode.S))
            {
                MoveCamera(Vector3.back * 3f);
            }
            if (Input.GetKey(KeyCode.D))
            {
                MoveCamera(Vector3.right * 3f);
            }
        }

        void MoveCamera(Vector3 v)
        {
            m_CameraPrefab.transform.Translate(v * Time.deltaTime);
        }
    }
}