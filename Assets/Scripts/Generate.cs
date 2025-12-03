using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Generate : MonoBehaviour
{
    // Start is called before the first frame update
    public int seed = 6;
    public int numCreatures = 1;
    public GameObject body;
    public GameObject wing1, wing2, wing3;
    public GameObject leg1, leg2, leg3;
    public GameObject mouth1, mouth2, mouth3;
    public Material wingMat1, wingMat2, wingMat3;
    public Material baseMaterial;
    void Start()
    {
        // for (int i = 0; i < numCreatures; i++)
        // {
        //     Random.InitState(seed+i);
        //     CreateCreature(new Vector3(20 * i, 0, 20 * i), new Vector3(0, 0, 0));
		// }
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public GameObject CreateCreature(Vector3 position, Vector3 rotation, bool hasWings = true)
    {
        GameObject creature = new GameObject("creature");
        CreatureAnimations refs = creature.AddComponent<CreatureAnimations>();
        List<RotateObj> legRotators = new List<RotateObj>();
        List<RotateObj> wingRotators = new List<RotateObj>();

        GameObject mainBody = Instantiate(body);
        mainBody.transform.SetParent(creature.transform, true);

    #region legs
        GameObject legLeftFront, legLeftMiddle, legLeftBack;
        GameObject legRightFront, legRightMiddle, legRightBack;
        int legType = Random.Range(1, 3);
        if (legType == 1)
        {
            legLeftFront = Instantiate(leg1);
            legLeftMiddle = Instantiate(leg1);
            legRightFront = Instantiate(leg1);
            legRightMiddle = Instantiate(leg1);
        }
        else
        {
            legLeftFront = Instantiate(leg2);
            legLeftMiddle = Instantiate(leg2);
            legRightFront = Instantiate(leg2);
            legRightMiddle = Instantiate(leg2);
        }

        if (Random.Range(0f, 1.0f) > 0.5f)
        {
            legLeftBack = Instantiate(leg3);
            legRightBack = Instantiate(leg3);
        }
        else
        {
            if (legType == 1)
            {
                legLeftBack = Instantiate(leg1);
                legRightBack = Instantiate(leg1);
            }
            else
            {
                legLeftBack = Instantiate(leg2);
                legRightBack = Instantiate(leg2);
            }
        }

        // legLeftFront;
        // legLeftMiddle; 
        // legLeftBack;
        // legRightFront;
        // legRightMiddle;
        // legRightBack;

        legLeftFront.transform.position = new Vector3(-1.5f, 0.5f, 0);
        legLeftMiddle.transform.position = new Vector3(0, 0, 0);
        legLeftBack.transform.position = new Vector3(1.5f, 0.5f, 0);
        
        legRightFront.transform.position = new Vector3(-1.5f, 0.5f, 0);
        legRightMiddle.transform.position = new Vector3(0, 0, 0);
        legRightBack.transform.position = new Vector3(1.5f, 0.5f, 0);

        if (!hasWings)
		{
            legLeftFront.transform.rotation = Quaternion.Euler(0f, -90f, -90f);
            legLeftMiddle.transform.rotation = Quaternion.Euler(0f, -120f, -90f);
            legLeftBack.transform.rotation = Quaternion.Euler(0f, -45f, -90f);
            
            legRightFront.transform.rotation = Quaternion.Euler(0f, 150f, -90f);
            legRightMiddle.transform.rotation = Quaternion.Euler(0f, 80f, -90f);
            legRightBack.transform.rotation = Quaternion.Euler(0f, 85f, -90f);

            RotateObj leftFront = legLeftFront.AddComponent<RotateObj>();
            RotateObj leftMiddle = legLeftMiddle.AddComponent<RotateObj>();
            RotateObj leftBack = legLeftBack.AddComponent<RotateObj>();

            RotateObj rightFront = legRightFront.AddComponent<RotateObj>();
            RotateObj rightMiddle = legRightMiddle.AddComponent<RotateObj>();
            RotateObj rightBack = legRightBack.AddComponent<RotateObj>();

            float randomSpeedWalk = Random.Range(4f, 8f);
            leftFront.xSpeed = randomSpeedWalk;
            leftFront.xAngle = 30.0f;
            leftMiddle.xSpeed = randomSpeedWalk;
            leftMiddle.xAngle = -20.0f;
            leftBack.xSpeed = randomSpeedWalk;
            leftBack.xAngle = 30.0f;

            rightFront.xSpeed = randomSpeedWalk;
            rightFront.xAngle = 30.0f;
            rightMiddle.xSpeed = randomSpeedWalk;
            rightMiddle.xAngle = -20.0f;
            rightBack.xSpeed = randomSpeedWalk;
            rightBack.xAngle = 20.0f;

            legRotators.Add(leftFront);
            legRotators.Add(leftMiddle);
            legRotators.Add(leftBack);

            legRotators.Add(rightFront);
            legRotators.Add(rightMiddle);
            legRotators.Add(rightBack);
        } else
		{
			legLeftFront.transform.rotation = Quaternion.Euler(0f, -80f, -90f);
            legLeftMiddle.transform.rotation = Quaternion.Euler(0f, -60f, -90f);
            legLeftBack.transform.rotation = Quaternion.Euler(0f, -45f, -90f);
            
            legRightFront.transform.rotation = Quaternion.Euler(0f, 80f, -90f);
            legRightMiddle.transform.rotation = Quaternion.Euler(0f, 60f, -90f);
            legRightBack.transform.rotation = Quaternion.Euler(0f, 45f, -90f);
		}

        legLeftFront.transform.SetParent(creature.transform, true);
        legLeftMiddle.transform.SetParent(creature.transform, true);
        legLeftBack.transform.SetParent(creature.transform, true);

        legRightFront.transform.SetParent(creature.transform, true);
        legRightMiddle.transform.SetParent(creature.transform, true);
        legRightBack.transform.SetParent(creature.transform, true);
    #endregion

    #region Mouth
        int mouthType = Random.Range(1, 4);
        if (mouthType == 3)
        {
            // Tube mouth, special case
            GameObject mouth = Instantiate(mouth3);
            mouth.transform.position = new Vector3(-6f, 1f, 0f);
            mouth.transform.rotation = Quaternion.Euler(90f, 0f, 90f);
            mouth.transform.SetParent(creature.transform, true);
        }
        else
        {
            GameObject leftMandible, rightMandible;
            if (mouthType == 1)
            {
                leftMandible = Instantiate(mouth1);
                rightMandible = Instantiate(mouth1);
            }
            else
            {
                leftMandible = Instantiate(mouth2);
                rightMandible = Instantiate(mouth2);
            }

            leftMandible.transform.position = new Vector3(-6f, 1f, 0.5f);
            rightMandible.transform.position = new Vector3(-6, 1, -0.5f);

            float mouthWidth = Random.Range(0.5f, 1.5f);
            float mouthLength = Random.Range(0.5f, 2f);
            leftMandible.transform.localScale = new Vector3(-1, mouthLength, mouthWidth);
            rightMandible.transform.localScale = new Vector3(1, mouthLength, mouthWidth);

            float angleShift = Random.Range(15f, 30f);
            leftMandible.transform.rotation = Quaternion.Euler(90f - angleShift, 0f, 90f - angleShift);
            rightMandible.transform.rotation = Quaternion.Euler(90f + angleShift, 0f, 90f + angleShift);

            leftMandible.transform.SetParent(creature.transform, true);
            rightMandible.transform.SetParent(creature.transform, true);
        }
    #endregion

        Color creatureColor = Random.ColorHSV(0.0f, 0.18f, 0.7f, 1.0f, 0.5f, 1.0f);
        Renderer[] allRenderers = creature.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in allRenderers)
		{
            rend.sharedMaterial = baseMaterial;
            rend.material.color = creatureColor;
		}

    #region wings
        if (hasWings)
		{
            // Wing position at point y = 5
            int wingType = Random.Range(1, 4);
            GameObject wingLeft, wingRight;
            if (wingType == 1)
            {
                wingLeft = Instantiate(wing1);
                wingRight = Instantiate(wing1);
            }
            else if (wingType == 2)
            {
                wingLeft = Instantiate(wing2);
                wingRight = Instantiate(wing2);
            }
            else
            {
                wingLeft = Instantiate(wing3);
                wingRight = Instantiate(wing3);
            }

            wingLeft.transform.position = new Vector3(0, 5, 0);
            wingRight.transform.position = new Vector3(0, 5, 0);
            float wingScale = Random.Range(1.0f, 5.0f);
            wingLeft.transform.localScale = new Vector3(wingScale, wingScale, -1);
            wingRight.transform.localScale = new Vector3(wingScale, wingScale, 1);

            RotateObj leftFlapper = wingLeft.AddComponent<RotateObj>();
            RotateObj rightFlapper = wingRight.AddComponent<RotateObj>();

            float randomSpeed = Random.Range(4f, 8f);
            leftFlapper.xSpeed = randomSpeed;
            rightFlapper.xSpeed = randomSpeed;
            leftFlapper.xAngle = -45.0f;
            rightFlapper.xAngle = 45.0f;

            wingRotators.Add(leftFlapper);
            wingRotators.Add(rightFlapper);

            wingLeft.transform.SetParent(creature.transform, true);
            wingRight.transform.SetParent(creature.transform, true);

            Color wingColor = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.8f, 1f);
            int wingDesign = Random.Range(1, 4);
            Renderer[] rend1 = wingLeft.GetComponentsInChildren<Renderer>();
            Renderer[] rend2 = wingRight.GetComponentsInChildren<Renderer>();
            if (wingDesign == 1)
            {
                foreach (Renderer rend in rend1)
                {
                    rend.sharedMaterial = wingMat1;
                    rend.material.color = wingColor;
                }
                foreach (Renderer rend in rend2)
                {
                    rend.sharedMaterial = wingMat1;
                    rend.material.color = wingColor;
                }
            }
            else if (wingDesign == 2)
            {
                foreach (Renderer rend in rend1)
                {
                    rend.sharedMaterial = wingMat2;
                    rend.material.color = wingColor;
                }
                foreach (Renderer rend in rend2)
                {
                    rend.sharedMaterial = wingMat2;
                    rend.material.color = wingColor;
                }
            }
            else
            {
                foreach (Renderer rend in rend1)
                {
                    rend.sharedMaterial = wingMat3;
                    rend.material.color = wingColor;
                }
                foreach (Renderer rend in rend2)
                {
                    rend.sharedMaterial = wingMat3;
                    rend.material.color = wingColor;
                }
            }
        }
    #endregion

        creature.transform.position = position;
        float creatureScale = Random.Range(0.5f, 0.75f);
        creature.transform.localScale = new Vector3(creatureScale, creatureScale, creatureScale);

    #region trails
        TrailRenderer trail = creature.AddComponent<TrailRenderer>();
        trail.time = 2.0f;
        trail.startWidth = 0.5f; 
        trail.endWidth = 0.0f;

        Gradient gradient = new Gradient();

        GradientColorKey[] trailColor;
        if (hasWings)
		{
			trailColor = new GradientColorKey[] { new GradientColorKey(Color.cyan, 0.0f), new GradientColorKey(Color.blue, 1.0f) };
		} else
		{
			trailColor = new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.blue, 1.0f) };
		}

        gradient.SetKeys(
            trailColor,
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        trail.colorGradient = gradient;

        // MATERIAL: Essential! Without a material, it will look pink/magenta.
        // "Sprites/Default" is a safe, built-in shader for simple colored lines.
        trail.material = new Material(Shader.Find("Sprites/Default"));
    #endregion

        refs.legScripts = legRotators.ToArray();
        refs.wingScripts = wingRotators.ToArray();

        return creature;
	}
}
