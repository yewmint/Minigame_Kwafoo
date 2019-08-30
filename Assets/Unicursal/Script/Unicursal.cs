using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unicursal : MonoBehaviour
{
    public GameObject DotPrefab;

    private Transform DotPanelTransform;
    private ParticleSystem TraceParticle;

    private Queue<GameObject> Dots = new Queue<GameObject>();
    private bool IsDragging = false;

    IEnumerator ShowDots()
    {
        for (int i = Dots.Count - 1; i >= 0; i--)
        {
            var dot = Dots.Dequeue();
            dot.GetComponent<Animator>().SetTrigger("Resume");
            Dots.Enqueue(dot);
            yield return new WaitForSeconds(0.3f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DotPanelTransform = transform.Find("DotPanel");
        TraceParticle = transform.Find("Trace Particle System").GetComponent<ParticleSystem>();

        // random positions
        for (int i = 0; i < 4; i++)
        {
            Vector3 dotPos = new Vector3(Random.Range(1, 7), Random.Range(1, 3), transform.position.z);
            if (i / 2 == 1) dotPos.y *= -1;
            if (i == 1 || i == 2) dotPos.x *= -1;

            GameObject dotObj = Instantiate(DotPrefab, dotPos, Quaternion.identity);
            dotObj.transform.parent = DotPanelTransform;
            Dots.Enqueue(dotObj);
        }

        StartCoroutine(ShowDots());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            IsDragging = true;
            TraceParticle.Play();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            IsDragging = false;
            TraceParticle.Stop();
        }

        if (IsDragging)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            TraceParticle.transform.position = new Vector3(mousePos.x, mousePos.y, TraceParticle.transform.position.z);

            var dot = Dots.Peek();
            float distance = Vector2.Distance(mousePos, dot.transform.position);

            if (distance < 0.5f)
            {
                dot = Dots.Dequeue();
                dot.GetComponent<Animator>().SetTrigger("Paint");
            }
        }
    }
}
