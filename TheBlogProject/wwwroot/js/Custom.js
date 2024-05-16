let index = 0;

function AddTag() {
    // Get a reference to the TagEntry input element
    var tagEntry = document.getElementById("TagEntry");
    // Lets use the new search function to help detect an error state
    let searchResult = search(tagEntry.value);
    if (searchResult != null){
        // Trigger my sweet alert for whatever condition is contained in the searchResult var
        swalWithDarkButton.fire({ 
            // html: `${searchResult.toUpperCase()}`,
            html: `<span><b>${searchResult.toUpperCase()}</b></span>`
        });
    }
    else {
        // Create a new Select Option
        let newOption = new Option(tagEntry.value, tagEntry.value);
        document.getElementById("TagValues").options[index++] = newOption; 
    }
    //Clear out the TagEntry control
    tagEntry.value = "";
    return true;
}
function DeleteTag() {
    let tagCount = 1;
    let tagList = document.getElementById("TagValues");
    if (!tagList) return false;
    
    if (tagList.selectedIndex === -1){
        swalWithDarkButton.fire({
            html: `<span>CHOOSE A TAG BEFORE DELETING</span>`
        });
        return true;
    }
    while (tagCount > 0) {
        if (tagList.selectedIndex >= 0) {
            tagList.options[tagList.selectedIndex] = null;
            --tagCount;
        } else
            tagCount = 0;
        index--;
    }
}
function ReplaceTag(tag, index){
    let newOption = new Option(tag, tag);
    document.getElementById("TagValues").options[index] = newOption;
}

// The Search function will detect either an empty or a duplicate Tag
// and return an error string if an error is detected
function search(str){
    if (str === ""){
        return 'Empty tags are not permitted';
    }
    var tagsElement = document.getElementById("TagValues");
    if (tagsElement){
        let options= tagsElement.options;
        for (let index = 0; index < options.length; index++){
            if (options[index].value === str)
                return `The Tag #${str} was detected as a duplicate and are not permitted`
        }
    }
}
$('form').on("submit", function () {
    $("#TagValues option").prop("selected", "selected");
})

// Look for the tagValues variable to see if it has data
if (tagValues != ''){
    let tagArray = tagValues.split(",");
    for (let loop = 0; loop < tagArray.length; loop++){
        // Load up old Replace the options that we have
        ReplaceTag(tagArray[loop], loop);
        index++
    }
}

const swalWithDarkButton = Swal.mixin({
    customClass:{
        confirmButton: 'btn btn-danger btn-sm w-100 btn-outline-dark'
    },
    imageUrl: '/img/oops.jpg',
    timer: 4000,
    buttonsStyling: false
});

