using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float _speed;
        float _fireTime;
        WorldCharacter _instigator;
        public void Init(Vector3 startLocation, DirectionEnum direction,WorldCharacter instigator)
        {
            _instigator = instigator;

            gameObject.SetActive(true);
            transform.position = startLocation;
            transform.eulerAngles = new Vector3(0f, CoordinateUtil.GetAngle(direction), 0f);
            _fireTime = Time.time;
        }
        private void Update()
        {
            transform.Translate(Vector3.forward * _speed * Game.GameTime.GetDeltaTime(), Space.Self);

            if(Time.time - _fireTime > 5f)
            {
                Destroy();
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (null == other.gameObject)
                return;
            PlayerCharacter playerChacater = other.GetComponent<PlayerCharacter>();
            if (null == playerChacater)
                return;

            if(Game.World.CurrentStage.TrySendDamage(new DamageInfo(_instigator, playerChacater, 1)))
            {
                Destroy();
            }
        }
        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}