using System;
using System.Collections.Generic;

namespace DbMocker.Tests.SampleTypes
{
    public class NavigationModel : SimpleModel
    {
        public ICollection<RelatedModel> RelatedModels { get; set; }

        public static new NavigationModel RandomInstance()
        {
            var model = SimpleModel.RandomInstance() as NavigationModel;
            model.RelatedModels = new List<RelatedModel>()
			{
				new RelatedModel() { Id = 1, Name = "Related 1" },
				new RelatedModel() { Id = 2, Name = "Related 2" },
				new RelatedModel() { Id = 3, Name = "Related 3" },
			};

            return model;
        }
    }
}
