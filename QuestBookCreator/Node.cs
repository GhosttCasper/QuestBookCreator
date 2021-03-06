﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace QuestBookCreator
{
    public interface Node
    {
        void Draw(Graphics g); //для обычных
        void DrawSpec(Graphics g); //для текущего
        void DrawSpec2(Graphics g); //для стартового
        float get_x();
        float get_y();
        void setContent(NodeContent c);
        NodeContent getContent();
        string get_name();
        int get_id();
        void set_name(string s);
        void set_x(float p);
        void set_y(float p);
        void refreshChildren();
    }
}