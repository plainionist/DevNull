
type Parent = {
    Name : string
    Age : int
    Children : Child list }
and Child = {
    Name : string
    Parent : Parent option }


module Builder = 
    let create name kids =
        let rec makeChild kid = { kid with Parent = parent |> Some }
        and parent = 
            {
                Name = name
                Age = 42
                Children = children
            }
        and children = kids |> List.map makeChild

        parent

    let createChild name =
        { Child.Name = name; Parent = None }

// compiles with warning and fails at runtime
//module Builder2 = 
//    let createAdult name age = 
//        { Parent.Name = name; Age = age; Children = [] }

//    let create name kids =
//        let rec makeChild kid = { kid with Parent = parent |> Some }
//        and parent = 
//            { (createAdult name 42) with
//                Children = children
//            }
//        and children = kids |> List.map makeChild

//        parent

//    let createChild name =
//        { Child.Name = name; Parent = None }

// as proposed on https://github.com/Microsoft/visualfsharp/issues/4201
module Builder3 =
    let createAdult name age = 
        { Parent.Name = name; Age = age; Children = [] }

    let create name kids =
        let rec makeChild kid = { kid with Parent = parent |> Some }
        and adult = createAdult name 42
        and children = kids |> List.map makeChild
        and parent = { adult with Children = children }
        parent

// as proposed on https://github.com/Microsoft/visualfsharp/issues/4201
module BuilderEvenSimpler =
    let createAdult name age = 
        { Parent.Name = name; Age = age; Children = [] }

    let makeChild parent kid = { kid with Parent = parent |> Some }

    let create name kids =
        let adult = createAdult name 42
        let children = kids |> List.map (makeChild adult)
 
        { adult with Children = children }

[<EntryPoint>]
let main argv = 
    let max = Builder.createChild "Max"
    let sarah = Builder.createChild "Sarah"

    let dad = Builder3.create "Peter" [ max; sarah ]

    printfn "Dad's name: %s" dad.Name
    printfn "Dad's #children: %i" dad.Children.Length
    
    // Fails :(
    printfn "Dad of first child: %A" dad.Children.[0].Parent
    printfn "Dad of second child: %A" dad.Children.[1].Parent

    0