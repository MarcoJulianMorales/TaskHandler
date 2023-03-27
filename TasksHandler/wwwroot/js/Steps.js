function AddStepHandle() {
    const indice = TaskEditVM.steps().findIndex(p => p.isNew());
    if (indice !== -1) {
        return; 
    }
    TaskEditVM.steps.push(new StepViewModel({ editMode: true, done: false }));
    $("[name=txtStepDescription]:visible").focus();
}

function CancelStep(step) {
    if (step.isNew()) {
        TaskEditVM.steps.pop();
    } else {
        step.editMode(false);
        step.description(step.prevDescription);
    }
}

async function SaveStep(step) {
    step.editMode(false);
    const isNew = step.isNew();
    const idTask = TaskEditVM.id;
    const data = getStepPetitionBody(step);

    const description = step.description();

    if (!description) {
        step.description(step.prevDescription);

        if (isNew) {
            TaskEditVM.steps.pop();
        }
        return;
    }

    if (isNew) {
        await insertStep(step, data, idTask);
    } else {
        UpdateStep(data, step.id());
    }
}

async function insertStep(step, data, idTask) {
    const response = await fetch(`${urlSteps}/${idTask}`, {
        body: data,
        method: "POST",
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (response.ok) {
        const json = await response.json();
        step.id(json.id)
    } else {
        HandleErrorApi(response)
    }
}

function getStepPetitionBody(step) {
    return JSON.stringify({
        description: step.description(),
        done: step.done()
    });
}

function EditStepDescription(step) {
    step.editMode(true);
    step.prevDescription = step.description();
    $("[name=txtStepDescription]:visible").focus();
}

async function UpdateStep(data, id) {
    const response = await fetch(`${urlSteps}/${id}`, {
        body: data,
        method: "PUT",
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (!response.ok) {
        HandleErrorApi(response);
    }
}

function ClickCheckBoxStep(step) {
    if (step.isNew()) {
        return true;
    }

    const data = getStepPetitionBody(step);
    UpdateStep(data, step.id());

    return true;
}