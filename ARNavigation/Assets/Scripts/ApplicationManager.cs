using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    public GameObject miniMap;
    public TMP_Dropdown navigationTargetDropdown;
    public List<Transform> navigationTargetObjects = new List<Transform>();
    public GameObject StartMenuPanel;
    public SetNavigationTarget setNavigation;
    
    private Transform currentTarget;


	// Start is called before the first frame update
	void Start()
	{

	}

    public void StartScan()
	{
        StartMenuPanel.SetActive(false);
        GetComponent<QRCodeRecenter>().ToggleScan();
	}

	public void SetCurrentNavigationTarget(int selectedValue)
    {
        if (selectedValue == 0)
            setNavigation.targetPosition = Vector3.zero;
        else
        {
            setNavigation.targetPosition = Vector3.zero;
            string selectedText = navigationTargetDropdown.options[selectedValue].text;
            if (currentTarget != null)
            {
                currentTarget.GetChild(0).gameObject.SetActive(false);
                currentTarget.GetChild(1).gameObject.SetActive(false);
            }
            currentTarget = navigationTargetObjects.Find(x => x.gameObject.name.Equals(selectedText));
            //Debug.Log(currentTarget.gameObject.name);
            if (currentTarget != null)
            {
                setNavigation.targetPosition = currentTarget.position;
                currentTarget.GetChild(0).gameObject.SetActive(true);
                currentTarget.GetChild(1).gameObject.SetActive(true);
            }
        }
    }
}
