module Fake.AWS.Profiles


let printProfileName () = 
    printf "Profile name: %s" Configuration.credentials.ProfileName

