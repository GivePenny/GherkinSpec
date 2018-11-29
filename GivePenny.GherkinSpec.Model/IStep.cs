﻿namespace GivePenny.GherkinSpec.Model
{
    public interface IStep
    {
        IStep CreateAnother(string newTitle);
        string Title { get; }
        string TitleAfterType { get; }
    }
}