using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace P_CMS.UtilityClasses
{
    public enum StatusType
    {
        Unassigned,Process,Assigned,Reassigned,Pending,Completed,Closed,ReAssignedByManager,ReAssignedByTUser,ReAssignedBySuperUser
    }
    public enum PriorityType
    {
        High,Medium,Low
    }
    public enum UserLoggedActions : byte
    {
       UserUpadated,RoleReassigned,UserRegisteredRoleAssigned,PasswordChanged,PasswordReset
    }    
    public enum CityNames
    {
       Karachi,Lahore,Islamabad
    }
    public enum CountryNames
    {       
        Pakistan
    }
  
}
