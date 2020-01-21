// This file is badly formatted on purpose
module Sample.Greeting

let say (name :     string) :   string =
    let name = 
        match name with
        | "" ->           "World"
        | name -> name
    sprintf        "Hello %s" name