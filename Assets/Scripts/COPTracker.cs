using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ReadWriteCSV;
using System.IO;

public class COPTracker : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Text>().text = "Tracking COP";
		
	}
	
	// Update is called once per frame
	void Update () {
        recordContinuous(Time.time, CoPtoCM(Wii.GetCenterOfBalance(0)));
	}

    // stores the data for writing to file at end of task
    List<ContinuousData> continuousData = new List<ContinuousData>();

    // participant id used for saving the file
    private string pid = "default";

    /// <summary>
    /// Write all data to a file
    /// </summary>
    void OnDisable()
    {
        WriteContinuousFile();
    }

    // Saves the participant id when typed in the text box
    public void SaveParticipantId(string arg0)
    {
        pid = arg0;
    }

    // Records continuous data to list
    public void recordContinuous(float time, Vector2 CoP)
    {
        continuousData.Add(new ContinuousData(time, CoP));
    }


    // Class that stores continuous data like CoP
    class ContinuousData
    {
        public readonly float time;
        public readonly Vector2 CoP;

        public ContinuousData(float time, Vector2 CoP)
        {
            this.time = time;
            this.CoP = CoP;
        }
    }

    /// <summary>
    /// Writes the Continuous File to a CSV
    /// </summary>
    private void WriteContinuousFile()
    {

        // Write all entries in data list to file
        Directory.CreateDirectory(@"Data/" + pid);
        using (CsvFileWriter writer = new CsvFileWriter(@"Data/" + pid + "/" + pid + ".csv"))
        {
            Debug.Log("Writing continuous data to file");

            // write header
            CsvRow header = new CsvRow();
            header.Add("Time");
            header.Add("COP X (cm)");
            header.Add("COP Y (cm)");

            writer.WriteRow(header);

            // write each line of data
            foreach (ContinuousData d in continuousData)
            {
                CsvRow row = new CsvRow();

                row.Add(d.time.ToString());
                row.Add(d.CoP.x.ToString());
                row.Add(d.CoP.y.ToString());

                writer.WriteRow(row);
            }
        }
    }

    /// <summary>
    /// Converts COP ratio to be in terms of cm.
    /// </summary>
    /// <param name="posn"> The current COB posn, not in terms of cm </param>
    /// <returns> The posn, in terms of cm </returns>
    public static Vector2 CoPtoCM(Vector2 posn)
    {
        return new Vector2(posn.x * 43.3f / 2f, posn.y * 23.6f / 2f);
    }

}