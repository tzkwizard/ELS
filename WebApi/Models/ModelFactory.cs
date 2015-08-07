
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Web;
using System.Web.Http.Routing;
using WebApi.App_Data;
using WebApi.App_Data.Entities;

namespace WebApi.Models
{
    public class ModelFactory
    {
        private UrlHelper _urlHelper;
        private ICountingKsRepository _repo;

        public ModelFactory(HttpRequestMessage request,ICountingKsRepository repo)
        {
            _urlHelper = new UrlHelper(request);
            _repo = repo;
        }
        public FoodModel Create(Food food)
        {
            return new FoodModel()
            {
                Url = _urlHelper.Link("Food",new{foodid=food.Id}),
                Description = food.Description,
                Measures = food.Measures.Select(m => Create(m))
            };
        }


        public DiaryModel Create(Diary diary)
        {
            return new DiaryModel()
            {
                Url = _urlHelper.Link("Diarires", new { diaryid = diary.CurrentDate.ToString("yyyy-MM-dd") }),
                CurrentDate = diary.CurrentDate,
                Entries = diary.Entries.Select(m=>Create(m))
            };
        }

        public DiaryEntryModel Create(DiaryEntry diaryEntry)
        {
            return new DiaryEntryModel()
            {
                Url = _urlHelper.Link("DiariresEntries", new { diaryid = diaryEntry.Diary.CurrentDate.ToString("yyyy-MM-dd"), id = diaryEntry.Id }),
                FoodDescription = diaryEntry.FoodItem.Description,
                MeasureDescription =diaryEntry.Measure.Description,
                MeasureUrl = _urlHelper.Link("Measures", new { foodid = diaryEntry.Measure.Food.Id, id = diaryEntry.Measure.Id }),
                Quantity=diaryEntry.Quantity
            };
        }




        public MeasureModel Create(Measure measure)
        {
            return new MeasureModel()
            {
                Url = _urlHelper.Link("Measures", new { foodid = measure.Food.Id,id=measure.Id }),
                Description = measure.Description,
                Calories = Math.Round(measure.Calories)
            };
        }

        public DiaryEntry Parse(DiaryEntryModel model)
        {
            try
            {
                var entry = new DiaryEntry();
                if (model.Quantity != default(double))
                {
                    entry.Quantity = model.Quantity;
                }

                if (!string.IsNullOrWhiteSpace(model.MeasureUrl))
                {
                    var uri = new Uri(model.MeasureUrl);
                var measureId = int.Parse(uri.Segments.Last());
                var measure = _repo.GetMeasure(measureId);
                entry.Measure = measure;
                entry.FoodItem = measure.Food;
                }
                   
            return entry;
            }
            catch 
            {

                return null;
            }
        }

        public MeasureV2Model Create2(Measure measure)
        {
            return new MeasureV2Model()
            {
                Url = _urlHelper.Link("Measures", new { foodid = measure.Food.Id, id = measure.Id }),
                Description = measure.Description,
                Calories = Math.Round(measure.Calories),
                TotalFat= measure.TotalFat,
                SaturatedFat =measure.SaturatedFat,
                Protein =measure.Protein,
                Carbohydrates =measure.Carbohydrates,
                Fiber =measure.Fiber,
                Sugar =measure.Sugar,
                Sodium =measure.Sodium,
                Iron =measure.Iron,
                Cholestrol =measure.Cholestrol
            };
        }
    }
}