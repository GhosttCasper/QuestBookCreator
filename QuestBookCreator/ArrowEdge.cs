using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace QuestBookCreator
{
    [Serializable()]
    class ArrowEdge : Edge
    {
        Node v1, v2;
        public Node get_v1()
        {
            return v1;
        }
        public Node get_v2()
        {
            return v2;
        }
        public void Draw(Graphics g)
        {
            Project curPr = Project.Instance();
            double x1 = v1.get_x() + curPr.nodeWidth;
            double y1 = v1.get_y() + curPr.nodeHeight / 2;
            double x2 = v2.get_x();
            double y2 = v2.get_y() + curPr.nodeHeight / 2;
            Pen p = new Pen(Brushes.DeepSkyBlue);
            g.DrawLine(p, (float)x1, (float)y1, (float)x2, (float)y2);
        }
        public ArrowEdge(Node vv1, Node vv2)
        {
            v1 = vv1;
            v2 = vv2;
        }
    }
}