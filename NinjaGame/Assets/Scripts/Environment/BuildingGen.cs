using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//execute in edit mode?


public class BuildingGen : MonoBehaviour {
    Mesh buildingExterior;
    //with exterior of building, fills interior

    //work by iteration

    //rooms generate for exterior(walls windows entrance), then hall/staircases for rooms+exterior, then change rooms to match, then stairs 

        //find windows and entrance
        //windows transparent mat?

    //from windows/entrance, get min size to cover, and max size to fit, rand(min,max)
        //check sightlines using raycast?
    
    //poisson disc sample pos other rooms

    //halls connect rooms/room doors connect rooms


    
    //use a* to find closest point if impossible to link, then reconnect from it



    //reinforce? least supported points


    
    

    //end loop

    ////////////////
    //layout/theme, logical structure
    //floors? flat xz planes?



}