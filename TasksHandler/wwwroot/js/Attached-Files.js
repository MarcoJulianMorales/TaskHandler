let inputTaskFile = document.getElementById('TaskFile');

function ClickAttachFile(){
    inputTaskFile.click();
}

async function SelectTaskFile(event) {
    const files = event.target.files;
    const filesArray = Array.from(files);

    const idTask = TaskEditVM.id;
    const formData = new FormData();
    for (var i = 0; i < filesArray.length; i++) {
        formData.append("files", filesArray[i]);
    }

    const response = await fetch(`${urlFiles}/${idTask}`, {
        body: formData,
        method: 'POST'
    });

    if (!response.ok) {
        HandleErrorApi(response);
        return;
    }

    const json = await response.json();
    console.log(json);
    prepareAttachedFiles(json);

    inputTaskFile.value = null;
}

function prepareAttachedFiles(attachedFiles) {
    attachedFiles.forEach(attachedFile => {
        let creationDate = attachedFile.creationDate;
        if (attachedFile.creationDate.indexOf('Z') === -1) {
            creationDate += 'Z';
        }

        const creationDateDT = new Date(creationDate);
        attachedFile.published = creationDateDT.toLocaleString();

        TaskEditVM.attachedFiles.push(
            new attachedFileVM({ ...attachedFile, editMode: false }));
    });
}