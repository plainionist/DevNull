open System

type Parent = {
    Name : string
    Age : int
    Children : Child list }
and Child = {
    Name : string
    Parent : Parent option }


// works but does not use "with" keyword
module ManualCopy = 
    let private createChild name =
        { Child.Name = name; Parent = None }

    let private create name kids =
        let rec makeChild kid = { kid with Parent = parent |> Some }
        and parent = 
            {
                Name = name
                Age = 42
                Children = children
            }
        and children = kids |> List.map makeChild

        parent

    let build() = 
        let max = createChild "Max"
        let sarah = createChild "Sarah"

        create "Peter" [ max; sarah ]

// compiles with warning and fails at runtime
module WithCopyButFS0040 = 
    let private createChild name =
        { Child.Name = name; Parent = None }

    let private createAdult name age = 
        { Parent.Name = name; Age = age; Children = [] }

    let private create name kids =
        let rec makeChild kid = { kid with Parent = parent |> Some }
        and parent = 
            { (createAdult name 42) with
                Children = children
            }
        and children = kids |> List.map makeChild

        parent

    let build() = 
        let max = createChild "Max"
        let sarah = createChild "Sarah"

        create "Peter" [ max; sarah ]

// as proposed on https://github.com/Microsoft/visualfsharp/issues/4201
module Issue4201Proposal =
    let private createChild name =
        { Child.Name = name; Parent = None }

    let private createAdult name age = 
        { Parent.Name = name; Age = age; Children = [] }

    let private create name kids =
        let rec makeChild kid = { kid with Parent = parent |> Some }
        and adult = createAdult name 42
        and children = kids |> List.map makeChild
        and parent = { adult with Children = children }
        parent

    let build() = 
        let max = createChild "Max"
        let sarah = createChild "Sarah"

        create "Peter" [ max; sarah ]

// as proposed on https://github.com/Microsoft/visualfsharp/issues/4201
module Issue4201ProposalSimpler =
    let private createChild name =
        { Child.Name = name; Parent = None }

    let createAdult name age = 
        { Parent.Name = name; Age = age; Children = [] }

    let makeChild parent kid = { kid with Parent = parent |> Some }

    let build() = 
        let max = createChild "Max"
        let sarah = createChild "Sarah"

        let peter = createAdult "Peter" 42
        let children = [max; sarah] |> List.map (makeChild peter)
 
        { peter with Children = children }

 module TwoPassLinking =
    let private createChild name =
        { Child.Name = name; Parent = None }

    let createAdult name age = 
        { Parent.Name = name; Age = age; Children = [] }

    let rec relinkParent (parent:Parent) =
        let rec createChild x = { x with Parent = p |> Some }
        and p = 
            {   Parent.Name = parent.Name
                Parent.Age = parent.Age
                Children = children }
        and children = parent.Children |> List.map createChild
        p

    let build() = 
        let max = createChild "Max"
        let sarah = createChild "Sarah"

        let peter = createAdult "Peter" 42
        
        { peter with Children = [ max; sarah ] }
        |> relinkParent

[<EntryPoint>]
let main argv = 

    let verify name (f:unit->Parent) =
        let isSame = LanguagePrimitives.PhysicalEquality 
        let parentOf child = child.Parent |> Option.get
        try
            printfn ""
            printfn "Verifying '%s'" name 

            let dad = f()

            printfn "  Dad's name          : %s  --> %b" dad.Name (dad.Name = "Peter")
            printfn "  Dad's #children     : %i  --> %b" dad.Children.Length (dad.Children.Length = 2)
            printfn "  Dad of first child  : %A  --> %b" (parentOf dad.Children.[0]).Name (isSame (parentOf dad.Children.[0]) dad)
            printfn "  Dad of second child : %A  --> %b" (parentOf dad.Children.[1]).Name (isSame (parentOf dad.Children.[1]) dad)
        with
            | ex -> printfn "  FAILED: '%s'" ex.Message

        printfn "======================================================="

    fun () -> ManualCopy.build() 
    |> verify "Manual copy"

    fun () -> WithCopyButFS0040.build() 
    |> verify "'with' copy but FS0040"

    fun () -> Issue4201Proposal.build() 
    |> verify "Proposal from Issue 4201"

    fun () -> Issue4201ProposalSimpler.build() 
    |> verify "Proposal from Issue 4201 (simpler)"

    fun () -> TwoPassLinking.build() 
    |> verify "Copy with 'with' and update back links in second pass"

    0