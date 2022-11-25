#include "VisitTask.h"

#include <utility>

#include "CreditFightTask.h"
#include "Sub/ProcessTask.h"

asst::VisitTask::VisitTask(AsstCallback callback, void* callback_arg)
    : PackageTask(std::move(callback), callback_arg, TaskType),
      m_visit_task_ptr(std::make_shared<ProcessTask>(m_callback, m_callback_arg, TaskType)),
      m_credit_fight_task_ptr(std::make_shared<CreditFightTask>(callback, callback_arg, TaskType))
{
    m_visit_task_ptr->set_tasks({ "VisitBegin" });
    m_subtasks.emplace_back(m_visit_task_ptr)->set_ignore_error(false);

    m_credit_fight_task_ptr->set_subtasks(m_subtasks);
    m_credit_fight_task_ptr->set_subtasks_enable(false);
}

bool asst::VisitTask::set_params(const json::value& params)
{
    bool credit_fight_task_enabled = params.get("credit_fight_task_enabled", false);

    m_credit_fight_task_ptr->set_subtasks_enable(credit_fight_task_enabled);
    
    return true;
}
