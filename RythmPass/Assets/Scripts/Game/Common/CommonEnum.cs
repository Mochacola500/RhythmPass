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
        Clear,              //클리어만해도 점수 획득
        TryCount,           //특정 횟수 이하로 시도해서 성공해야 점수 획득
        GetScoreItem,       //점수 얻는 아이템 획득해야 점수 획득
        Time,               //특정 시간내로 성공해야 점수 획득
        PathCount,          //총 이동 칸 수 특정 횟수 이하여야 성공
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