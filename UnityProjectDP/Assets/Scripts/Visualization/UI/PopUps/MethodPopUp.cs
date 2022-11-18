using UnityEngine;
using TMPro;
using AnimArch.Visualization.Diagrams;
using OALProgramControl;

namespace AnimArch.Visualization.UI
{
    public class MethodPopUp : AbstractPopUp
    {
        public TMP_Dropdown dropdown;
        private TMP_Text _mtdTxt;

        public void ActivateCreation(TMP_Text classTxt, TMP_Text mtdTxt)
        {
            ActivateCreation(classTxt);
            _mtdTxt = mtdTxt;
        }

        public override void Confirmation()
        {
            if (inp.text == "")
            {
                Deactivate();
                return;
            }
            var mtd = new Method
            {
                Name = inp.text,
                ReturnValue = dropdown.options[dropdown.value].text
            };

            // var tempCdClass = DiagramPool.Instance.ClassDiagram.FindClassByName(ClassTxt.text).ClassInfo;
            // mtd.Name = mtd.Name.Replace(" ", "_");
            // var cdMethod = new CDMethod(tempCdClass, mtd.Name, EXETypes.ConvertEATypeName(mtd.ReturnValue));
            // tempCdClass.AddMethod(cdMethod);
            
            // foreach (var arg in mtd.arguments)
            // {
            //     var tokens = arg.Split(' ');
            //     var type = tokens[0];
            //     var mName = tokens[1];
            //
            //     Method.Parameters.Add(new CDParameter{ Name = mName, Type = EXETypes.ConvertEATypeName(type) });
            // }
            
            if (DiagramPool.Instance.ClassDiagram.AddMethod(ClassTxt.text, mtd))
            {
                _mtdTxt.text += mtd.Name + "() :" + mtd.ReturnValue + "\n";
            }

            Deactivate();
        }
    }
}