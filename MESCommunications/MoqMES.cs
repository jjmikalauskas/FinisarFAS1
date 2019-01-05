﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESCommunications
{
    public class MESService : IMESService
    {
        private IMESService _mesService; 

        public MESService(IMESService iService)
        {
            _mesService = iService; 
        }
        
        #region PUBLIC METHODS

        public Operator GetOperator(string operatorName, int id)
        {
            var op = _mesService.GetOperator(operatorName, id);
            return op; 
        }

        public Tool GetTool(string toolName, int id)
        {
            var tool = new Tool();
            tool.Id = GetNextRandom(toolName);
            if (tool.Id < 100)
                tool = null; 
            return tool;
        }

        public Lot GetLot(string lotName, int id)
        {
            var lot = new Lot();
            lot.Id = GetNextRandom(lotName);
            if (lot.Id < 100)
                lot = null;
            return lot;
        }

        #endregion

        private int GetNextRandom(string s)
        {
            int seed = 0; 
            foreach( char c in s)
            {
                seed += (int)c; 
            }
            Random r = new Random(seed);
            return r.Next(999); 
        }

    }

    public class MESDAL
    {
        public static List<Wafer> GetCurrentWaferSetup(int id)
        {
            var wafers = new List<Wafer>();
            for (int i = 1; i <= 25; ++i)
            {
                wafers.Add(new Wafer() { Slot = "Slot #" + i, WaferNo = id.ToString() + '-' + i.ToString(), Scrap = "No scrap " + i.ToString(), Recipe = "RecipeX" });
            }

            wafers[0].Status = "Completed";
            wafers[1].Status = "In Progress...";

            if (id==1)
            {
                wafers[0].Status = "Completed";
                wafers[1].Status = "Completed";
                wafers[2].Status = "Completed";
                wafers[3].Status = "Completed";
                wafers[4].Status = "Completed";
                wafers[5].Status = "In Progress...";
            }

            return wafers; 
        }
    }


}