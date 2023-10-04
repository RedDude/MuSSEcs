using System.Collections.Generic;


/*
 * Table of Label Equivalence.
 * 
 * The algorithm will reduce the labels to their corresponding representatives. 
 * A representative is given by the minimum value in the table of equivalence, 
 * i.e. minimum label id compared to surrounding neighbors.
 * 
 * The algorithm later creates a list for every blob in the selected area and 
 * match it accordingly to the reduced labels.
 * 
 * That will guaranty that all equivalent labels are assigned the same region 
 * value.
 * 
 * Example:
 * 	Label id 	Equivalent Labels
 * 		1 			1,6
 * 		2 			2,3,4,5
 * 		3 			2,3,4,5
 * 		4	 		2,3,4,5
 * 		5			2,3,4,5
 * 		6			1,6
 * 
 * 		As a result, the algorithm will reduce labels 1 and 6 to label 1, 
 * 		and labels 2, 3, 4 and 5, to label 2.
 * 
 */
namespace MuSSEcs.blobdetection
{
    class LabelEqTable
    {
        /*
     *  Table of Label Equivalence
     *  Map : Label id (parent) to Equivalent Labels (child)
     *  
     */
        private Dictionary<Label, Label> _eqMap;

        // Creates a new Table
        public LabelEqTable()
        {
            _eqMap = new();
        }

        // Set a Label Equivalence
        private void SetChild(Label parent, Label child)
        {
            _eqMap.Add(parent, child);
        }

        // Get equivalences for selected Label
        private Label GetChild(Label parent)
        {
            if (!HasLabel(parent))
            {
                // throw new NoSuchElementException("Parent label not yet added.")
            }

            return _eqMap[parent];
        }

        // Adds new Label to Table
        public bool AddLabel(Label lab)
        {
            // if (lab == null)
            // {
            //     // throw new NullPointerException("Cannot add null Labels.");
            // }

            var isNewLabel = !HasLabel(lab);
            if (isNewLabel)
            {
                // Every Label is Equivalent to itself
                SetChild(lab, lab);
            }

            return isNewLabel;
        }

        // Verifies if Table contains a specific Label.
        public bool HasLabel(Label lab)
        {
            return _eqMap.ContainsKey(lab);
        }

        /*
     *  Reduce Equivalences.
     *  
     *  Add a Label to existing Clip (representative) based on their equivalence.
     *  
     */
        public void SetComembers(Label first, Label second)
        {
            // First get the representative (origin Label) for each label.
            var firstRep = GetRep(first);
            var secondRep = GetRep(second);

            // Do nothing in case label has no equivalences besides itself
            if (firstRep.Equals(secondRep))
            {
                return;
            }

            // Sets the smaller Label as an equivalence to the higher Label
            var max = Max(firstRep, secondRep);
            var min = Min(firstRep, secondRep);
            SetChild(max, min);
        }

        // Recursively gets the smaller Label in the Equivalence sequence (origin Label).
        public Label GetRep(Label lab)
        {
            var child = GetChild(lab);
            if (child == lab)
            {
                return lab;
            }

            var rep = GetRep(child);
            SetChild(lab, rep);
            return rep;
        }

        // Return smaller Label in the pair
        Label Min(Label first, Label second)
        {
            return first.id < second.id ? first : second;
        }

        // Return higher Label in the pair
        Label Max(Label first, Label second)
        {
            return first.id > second.id ? first : second;
        }
    }
}