let index = 0;

function AddTag() {
    // Get a reference to the TagEntry input element
    var tagEntry = document.getElementById("TagEntry");
    // Create a new Select Option
    let newOption = new Option(tagEntry.value, tagEntry.value);
    document.getElementById("TagValues").options[index++] = newOption;
    //Clear out the TagEntry control
    tagEntry.value = "";
    return true;
}

function DeleteTag() {
    let tagCount = 1;
    while (tagCount > 0) {
        let tagList = document.getElementById("TagValues");
        let selectedIndex = tagList.selectedIndex;
        if (selectedIndex >= 0) {
            tagList.options[selectedIndex] = null;
            --tagCount;
        } else
            tagCount = 0;
        index--;
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

function ReplaceTag(tag, index){
    let newOption = new Option(tag, tag);
    document.getElementById("TagValues").options[index] = newOption;
}