window.calendarWeek = window.calendarWeek || {
    scrollDayIntoCenter: function (container, dayIndex) {
        if (!container || typeof dayIndex !== "number") {
            return;
        }

        const dayIndexSafe = Math.max(0, Math.min(6, dayIndex));

        requestAnimationFrame(() => {
            requestAnimationFrame(() => {
                if (container.scrollWidth <= container.clientWidth) {
                    return;
                }

                const timeColumnWidth = 50;
                const totalDaysWidth = Math.max(0, container.scrollWidth - timeColumnWidth);
                if (totalDaysWidth === 0) {
                    return;
                }

                const dayWidth = totalDaysWidth / 7;
                const dayCenterX = timeColumnWidth + (dayIndexSafe * dayWidth) + (dayWidth / 2);
                const targetScrollLeft = dayCenterX - (container.clientWidth / 2);

                const maxScrollLeft = Math.max(0, container.scrollWidth - container.clientWidth);
                container.scrollLeft = Math.min(Math.max(0, targetScrollLeft), maxScrollLeft);
            });
        });
    }
};
