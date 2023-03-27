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

    }
}

async function SaveStep(step) {
    step.editMode(false);
    const isNew = step.isNew();
    const idTask = TaskEditVM.id;
    const data = getStepPetitionBody(step);

    if (isNew) {
        await insertStep(step, data, idTask);
    } else {

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