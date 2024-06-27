#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

namespace Magio
{
    public class PlayerThrowMagic : MonoBehaviour
    {
        [System.Serializable]
        public struct MagicSpell
        {
            public string name;
            public EffectClass effectClass;
            [ColorUsageAttribute(true, true)]
            public Color magicColor;
            [ColorUsageAttribute(true, true)]
            public Color magicInnerColor;

            public MagioObjectEffect magioObj;

            public GameObject splashEffect;
        }

        public List<MagicSpell> spells;

        public Animator animator;
        public GameObject magic;
        public Camera cam;

        public Text weaponText;

        public Transform firePoint;
        float coolDown = 0;

        public int currentWeapon = 0;

        float particleMult;

#if ENABLE_INPUT_SYSTEM
        InputAction shoot;

        InputAction nextWeapon;
        InputAction previousWeapon;

        InputAction stopPlaying;

        void Start()
        {

            shoot = new InputAction("PlayerShoot", binding: "<Gamepad>/x");
            shoot.AddBinding("<Mouse>/leftButton");

            shoot.Enable();

            nextWeapon = new InputAction("NextWeapon", binding: "<Gamepad>/rb");
            nextWeapon.AddBinding("<Keyboard>/E");

            nextWeapon.Enable();

            previousWeapon = new InputAction("PreviousWeapon", binding: "<Gamepad>/lb");
            previousWeapon.AddBinding("<Keyboard>/Q");

            previousWeapon.Enable();

            stopPlaying = new InputAction("StopPlaying", binding: "<Gamepad>/b");
            stopPlaying.AddBinding("<Keyboard>/Escape");

            stopPlaying.Enable();

            stopPlaying.started += StopPlaying_started;
            nextWeapon.started += NextWeapon_started;
            previousWeapon.started += PreviousWeapon_started;

            animator.StopPlayback();

            spells[currentWeapon].magioObj.gameObject.SetActive(true);

            weaponText.text = "Spell: " + (currentWeapon + 1).ToString() + " - " + spells[currentWeapon].name;
        }

        private void StopPlaying_started(InputAction.CallbackContext obj)
        {
            Application.Quit();
        }


        private void PreviousWeapon_started(InputAction.CallbackContext obj)
        {
            spells[currentWeapon].magioObj.gameObject.SetActive(false);

            currentWeapon--;
            if (currentWeapon < 0)
            {
                currentWeapon = spells.Count - 1;
            }

            spells[currentWeapon].magioObj.gameObject.SetActive(true);
            weaponText.text = "Spell: " + (currentWeapon + 1).ToString() + " - " + spells[currentWeapon].name;
        }

        private void NextWeapon_started(InputAction.CallbackContext obj)
        {
            spells[currentWeapon].magioObj.gameObject.SetActive(false);
            currentWeapon++;
            if (currentWeapon >= spells.Count)
            {
                currentWeapon = 0;
            }

            spells[currentWeapon].magioObj.gameObject.SetActive(true);
            weaponText.text = "Spell: " + (currentWeapon + 1).ToString() + " - " + spells[currentWeapon].name;
        }


#else
        void Start()
        {

            animator.StopPlayback();

            spells[currentWeapon].magioObj.gameObject.SetActive(true);
            weaponText.text = "Spell: " + (currentWeapon + 1).ToString() + " - " + spells[currentWeapon].name;
        }
#endif

        // Update is called once per frame
        void Update()
        {
            bool shootPressed = false;

#if ENABLE_INPUT_SYSTEM
            shootPressed = Mathf.Approximately(shoot.ReadValue<float>(), 1);
#else
            shootPressed = Input.GetKeyDown(KeyCode.Mouse0);
            bool prevWeaponPressed = Input.GetKeyDown(KeyCode.Q);
            bool nextWeaponPressed = Input.GetKeyDown(KeyCode.E);
            bool stopPlayingPressed = Input.GetKeyDown(KeyCode.Escape);

            if (nextWeaponPressed)
            {
                spells[currentWeapon].magioObj.gameObject.SetActive(false);
                currentWeapon++;
                if (currentWeapon >= spells.Count)
                {
                    currentWeapon = 0;
                }
                spells[currentWeapon].magioObj.gameObject.SetActive(true);
                weaponText.text = "Spell: " + (currentWeapon + 1).ToString() + " - " + spells[currentWeapon].name;
            }
            else if (prevWeaponPressed)
            {
                spells[currentWeapon].magioObj.gameObject.SetActive(false);
                currentWeapon--;
                if (currentWeapon < 0)
                {
                    currentWeapon = spells.Count - 1;
                }

                spells[currentWeapon].magioObj.gameObject.SetActive(true);
                weaponText.text = "Spell: " + (currentWeapon + 1).ToString() + " - " + spells[currentWeapon].name;
            }

            if(stopPlayingPressed)
            {
                Application.Quit();
            }
#endif

            if (particleMult < 1)
            {
                particleMult += Time.deltaTime * 0.5f;
            }
            else
            {
                particleMult = 1;
            }

            if (coolDown >= 0)
            {
                coolDown -= Time.deltaTime;
            }

            if (shootPressed && coolDown <= 0)
            {
                animator.SetTrigger("Throw");
                coolDown = 2;

                Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

                RaycastHit hit;
                Vector3 destination = Vector3.zero;

                if (Physics.Raycast(ray, out hit))
                {
                    destination = hit.point;
                }
                else
                {
                    destination = ray.GetPoint(1000);
                }

                GameObject magicGO = Instantiate(magic, firePoint.position, Quaternion.identity);

                MagicImpact impact = magicGO.GetComponent<MagicImpact>();
                MagicMoveTowards magicMove = magicGO.GetComponent<MagicMoveTowards>();

                magicMove.target = destination;
                impact.color = spells[currentWeapon].magicColor;
                impact.innerColor = spells[currentWeapon].magicInnerColor;
                impact.effClass = spells[currentWeapon].effectClass;
                impact.splashEffect = spells[currentWeapon].splashEffect;

                particleMult = 0;
            }
        }
    }

}
