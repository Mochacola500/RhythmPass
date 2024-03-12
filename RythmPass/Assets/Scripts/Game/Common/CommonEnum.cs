using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    public enum AddressableError
    {
        UnknownError = -1,
        NotConnectedInternet = 10001,
        ConnectedInternetButSomethingError = 10002,
        NotExistLabel = 20001,
    }

    public enum DirectionEnum : int
    {
        Left,
        Right,
        Up,
        Down,
        None,
    }
    public enum StageScoreTypeEnum : int
    {
        None = 0,
        Clear,              //Ŭ����ص� ���� ȹ��
        TryCount,           //Ư�� Ƚ�� ���Ϸ� �õ��ؼ� �����ؾ� ���� ȹ��
        GetScoreItem,       //���� ��� ������ ȹ���ؾ� ���� ȹ��
        Time,               //Ư�� �ð����� �����ؾ� ���� ȹ��
        PathCount,          //�� �̵� ĭ �� Ư�� Ƚ�� ���Ͽ��� ����
    }
    public enum CurrencyEnum : int
    {
        None = 0,
        Coin,
        Dia,
        Ticket,
    }
    public enum RewardTypeEnum : int
    {
        None = 0,
        Currency = 1,
        Item = 2,
    }
    public enum StageEnterTypeEnum : int
    {
        Currency,
        Admob,
    }
    public static class CommonEnumExtentions
    {
        public static Vector2Int DirectionToIndex(this DirectionEnum direction)
        {
            switch (direction)
            {
                case DirectionEnum.Left:
                    return new Vector2Int(-1, 0);
                case DirectionEnum.Right:
                    return new Vector2Int(1, 0);
                case DirectionEnum.Down:
                    return new Vector2Int(0, -1);
                case DirectionEnum.Up:
                    return new Vector2Int(0, 1);
            }
            return new Vector2Int(0, 0);
        }
        public static DirectionEnum GetReverseDirection(this DirectionEnum direction)
        {
            switch (direction)
            {
                case DirectionEnum.Down:
                    return DirectionEnum.Up;
                case DirectionEnum.Up:
                    return DirectionEnum.Down;
                case DirectionEnum.Right:
                    return DirectionEnum.Left;
                case DirectionEnum.Left:
                    return DirectionEnum.Right;
            }

            return DirectionEnum.Down;
        }
    }
}