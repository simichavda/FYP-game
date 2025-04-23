using UnityEngine;
using UnityEngine.UI;

public class CopyToClipboard : MonoBehaviour
{
    public string linkToCopy = "https://your-link-here.com";
    public Text feedbackText; 

    public void CopyLink()
    {
        Debug.Log("Copying link to clipboard: " + linkToCopy);
        
        GUIUtility.systemCopyBuffer = linkToCopy;

        
        if (feedbackText != null)
        {
            feedbackText.text = "Copied to clipboard!";
            Invoke("ClearFeedback", 2f); 
        }
       
        Debug.Log("Copied To Clipboard");
    }

    void ClearFeedback()
    {
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }
    }
}