using AutoMapper;
using Resources.Base.Exception;
using Resources.Base.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeVeed.Application.Dtos;
using WeVeed.Domain.Entities;
using WeVeed.Domain.Services;

namespace WeVeed.Application.Services
{
    public class UserAppService : IUserAppService
    {
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;
        private readonly ISeriesService _seriesService;
        private readonly IVideoService _videoService;
        private readonly IFollowService _followService;
        private readonly IChannelService _channelService;
        private readonly ISeriesAppService _seriesAppService;
        private readonly IViewsFilterService _viewsFilterService;
        private static Random rng = new Random();

        public UserAppService(IUserService userService, IEmailSender emailSender, IFollowService followSevice, IChannelService channelService, ISeriesAppService seriesAppService,
            ISeriesService seriesService, IVideoService videoService, IViewsFilterService viewsFilterService)
        {
            _userService = userService;
            _emailSender = emailSender;
            _seriesService = seriesService;
            _videoService = videoService;
            _followService = followSevice;
            _channelService = channelService;
            _seriesAppService = seriesAppService;
            _viewsFilterService = viewsFilterService;
        }

        public async Task DeleteNotValidated()
        {
            var notActivatedUserIds = await _userService.GetNotActivatedUserIds();
            var activatedUserIds = await _userService.GetActivatedUserIds();

            foreach(var userId in notActivatedUserIds)
            {
                var series = await _seriesService.GetAllByProducer(userId);
                var videos = await _videoService.GetAllUserPaginatedAsync(userId, new AllVideoPaginateInput { Page = 1, PageSize = 200 });


                foreach(var video in videos)
                {
                    // delete video from channel 
                    await _channelService.DeleteVideoFromChannel(video.Id.ToString(), video.SeriesCategory);
                    // delete video 
                    await _videoService.DeleteAsync(video.Id.ToString());
                }

                foreach(var serie in series)
                {
                    // delete seriesId from FollowedList of all the remaining users 
                    foreach(var activatedUserId in activatedUserIds)
                    {
                        await _seriesAppService.UnFollowSeriesAsync(activatedUserId, new SeriesFollowInput { SeriesId = serie.Id.ToString() });
                    }

                    // delete series 
                    await _seriesService.DeleteAsync(serie.Id.ToString());
                }

                // delete the user 
                await _userService.DeleteAsync(userId);
            }
        }

        #region Reset Password

        public async Task<bool> SendResetPasswordEmailAsync(ResetPasswordSendEmailInput input)
        {
            var user = await _userService.GetByEmailAsync(input.Email);
            if(user == null)
            {
                return false;
            }

            var resetToken = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();

            // set on the user object the reset token and date 
            var wasResetInfoSet = await _userService.SetResetPasswordInformationAsync(user.Id.ToString(), resetToken);
            if (!wasResetInfoSet)
            {
                return false;
            }

            var emailSent = _emailSender.SendEmailToResetPassword(input.Email, resetToken, user.FirstName + " " + user.LastName);

            return emailSent;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordInput input)
        {
            // validate the reset token + email combination
            var user = await _userService.GetByResetTokenAsync(input.ResetToken);
            if (user == null)
            {
                return false;
            }

            if (!user.IsResetTokenActive)
            {
                return false;
            }

            if((DateTime.Now - user.ResetDate).TotalHours > 2)
            {
                return false;
            }

            // if it got here, the reset token is valid, so change the password 
            var wasPasswordReset = await _userService.ResetPasswordAsync(user.Id.ToString(), input.NewPassword);

            return wasPasswordReset;
        }

        #endregion

        public async Task<bool> ValidateProducerByAdmin(ValidateProducerByAdminInput input)
        {
            var producer = await _userService.GetByIdAsync(input.ProducerId);

            if (producer == null)
            {
                return false;
            }

            // Set flag on User entity
            var wasUserValidated = await _userService.ValidateProducerByAdmin(input);
            if (!wasUserValidated)
            {
                return false;
            }

            // Set flag on Series entities 
            var wereSeriesValidated = await _seriesService.ValidateProducerSeriesByAdmin(input);

            // Set flag on Video entities 
            var wereVideosValidated = await _videoService.ValidateProducerVideosByAdmin(input);

            // Send email to producer to let him know his account is now validated 
            if (input.SendEmailToProducer)
            {
                _emailSender.SendProducerEmailAfterAdminValidateAccount(producer.Email.ToLower(), producer.ProducerName);
            }

            return true;
        }
        
        // DONE
        public async Task<List<ProducerListViewDto>> SearchProducerAsync(string word)
        {
            var producers = await _userService.SearchProducerAsync(word);
            if (producers == null)
            {
                return new List<ProducerListViewDto>();
            }

            return producers.Select(a => Mapper.Map<ProducerListViewDto>(a)).ToList();
        }

        // DONE
        public List<ProducerListViewDto> Shuffle(List<ProducerListViewDto> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                ProducerListViewDto value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        // DONE
        public async Task<List<ProducerListViewDto>> GetDiscoverProducersAsync()
        {
            var popularProducers = await GetMostPopularProducersAsync();
            var recentProducers = await GetMostRecentProducersAsync();

            popularProducers.ForEach(a =>
            {
                if (!recentProducers.Select(e => e.Id).Contains(a.Id))
                {
                    a.IsNew = false;
                    recentProducers.Add(a);
                }
            });

            return Shuffle(recentProducers);
        }

        // DONE
        public async Task<List<ProducerListViewDto>> GetMostPopularProducersAsync()
        {
            var producers = await _userService.GetMostPopularProducersAsync();
            if(producers == null)
            {
                return new List<ProducerListViewDto>();
            }

            return producers.Select(a => Mapper.Map<ProducerListViewDto>(a)).ToList();
        }

        // DONE
        public async Task<List<ProducerListViewDto>> GetMostRecentProducersAsync()
        {
            var producers = await _userService.GetMostRecentProducersAsync();
            if (producers == null)
            {
                return new List<ProducerListViewDto>();
            }

            return producers.Select(a => Mapper.Map<ProducerListViewDto>(a)).ToList();
        }

        // DONE
        public async Task<List<ProducerListViewDto>> GetDiscoverProducersFollowedWeekly()
        {
            var producersList = await _followService.GetTopProducersIdsWeekly();
            if (producersList == null || !producersList.Any())
            {
                return new List<ProducerListViewDto>();
            }

            var producers = await _userService.GetAllByIdsList(producersList, true);
            if (producers == null)
            {
                return new List<ProducerListViewDto>();
            }

            var sorted = producers.OrderBy(a => producersList.IndexOf(a.Id.ToString())).ToList();

            var prodDtos = sorted.Select(a => Mapper.Map<ProducerListViewDto>(a)).ToList();
            prodDtos.ForEach(a => a.IsNew = false);

            return prodDtos;
        }

        // DONE
        public async Task<List<ProducerListViewDto>> GetDiscoverProducersFollowedMonthly()
        {
            var producersList = await _followService.GetTopProducersIdsMonthly();
            if (producersList == null || !producersList.Any())
            {
                return new List<ProducerListViewDto>();
            }

            var producers = await _userService.GetAllByIdsList(producersList, true);
            if (producers == null)
            {
                return new List<ProducerListViewDto>();
            }

            var sorted = producers.OrderBy(a => producersList.IndexOf(a.Id.ToString())).ToList();

            var prodDtos = sorted.Select(a => Mapper.Map<ProducerListViewDto>(a)).ToList();
            prodDtos.ForEach(a => a.IsNew = false);

            return prodDtos;
        }

        // DONE
        public async Task<List<ProducerListViewDto>> GetDiscoverProducersFollowedAllTime()
        {
            var producersList = await _userService.GetMostFollowedProducers();
            if (producersList == null)
            {
                return new List<ProducerListViewDto>();
            }

            var prodDtos = producersList.Select(a => Mapper.Map<ProducerListViewDto>(a)).ToList();
            prodDtos.ForEach(a => a.IsNew = false);

            return prodDtos;
        }

        // DONE
        public async Task<List<ProducerListViewDto>> GetDiscoverMostRecentProducers()
        {
            var producersList = await _userService.GetDiscoverMostRecentProducersAsync();
            if (producersList == null)
            {
                return new List<ProducerListViewDto>();
            }

            return producersList.Select(a => Mapper.Map<ProducerListViewDto>(a)).ToList();
        }

        // DONE
        public async Task<List<ProducerListViewDto>> GetDiscoverProducersMostViewedWeekly()
        {
            var producersIds = await _viewsFilterService.GetMostViewedProducersIdsWeekly();
            if (producersIds == null || !producersIds.Any())
            {
                return new List<ProducerListViewDto>();
            }

            var producers = await _userService.GetAllByIdsList(producersIds, true);
            if (producers == null)
            {
                return new List<ProducerListViewDto>();
            }

            var sorted = producers.OrderBy(a => producersIds.IndexOf(a.Id.ToString())).ToList();

            var prodDtos = sorted.Select(a => Mapper.Map<ProducerListViewDto>(a)).ToList();
            prodDtos.ForEach(a => a.IsNew = false);

            return prodDtos;
        }

        // DONE
        public async Task<List<ProducerListViewDto>> GetDiscoverProducersMostViewedMonthly()
        {
            var producersIds = await _viewsFilterService.GetMostViewedProducersIdsMonthly();
            if (producersIds == null || !producersIds.Any())
            {
                return new List<ProducerListViewDto>();
            }

            var producers = await _userService.GetAllByIdsList(producersIds, true);
            if (producers == null)
            {
                return new List<ProducerListViewDto>();
            }

            var sorted = producers.OrderBy(a => producersIds.IndexOf(a.Id.ToString())).ToList();

            var prodDtos = sorted.Select(a => Mapper.Map<ProducerListViewDto>(a)).ToList();
            prodDtos.ForEach(a => a.IsNew = false);

            return prodDtos;
        }

        // DONE
        public async Task<List<ProducerListViewDto>> GetDiscoverProducersMostViewedAllTime()
        {
            var producersList = await _userService.GetMostViewedProducers();
            if (producersList == null)
            {
                return new List<ProducerListViewDto>();
            }

            var prodDtos = producersList.Select(a => Mapper.Map<ProducerListViewDto>(a)).ToList();
            prodDtos.ForEach(a => a.IsNew = false);

            return prodDtos;
        }

        public async Task<UserBasicInfoDto> GetBasicInfoById(string id)
        {
            var entity = await _userService.GetByIdAsync(id);
            return Mapper.Map<UserBasicInfoDto>(entity);
        }

        public async Task<UserBasicInfoDto> GetBasicInfoByFacebookId(string id)
        {
            var entity = await _userService.GetByFacebookIdAsync(id);
            if(entity == null)
            {
                return null;
            }

            return Mapper.Map<UserBasicInfoDto>(entity);
        }

        public async Task<ProducerViewDto> GetProducerViewDtoAsync(string producerId)
        {
            var user = await _userService.GetByIdAsync(producerId);
            if(user == null || user.UserType != "producer")
            {
                return null;
            }

            var dto = Mapper.Map<ProducerViewDto>(user);
            if(dto == null)
            {
                return null;
            }

            dto.NumberOfSeries = await _seriesService.CountProducerSeriesAsync(producerId);
            dto.NumberOfSeriesEpisodes = await _videoService.CountProducerEpisodesAsync(producerId);

            return dto;
        }

        public async Task<bool> RegisterAsync(UserRegisterInput input)
        {
            var registerGuid = Guid.NewGuid();
            var id = await _userService.CreateAsync(input, registerGuid);

            var confirmationEmailSent = _emailSender.SendRegisterConfirmationEmail(registerGuid, input.Email, input.FirstName);
            if (!confirmationEmailSent)
            {
                await _userService.DeleteAsync(id);
                return false;
            }

            // Send email to admin to let them know there is a new producer 
            if(input.UserType == "producer")
            {
                var emailToAdminSent = _emailSender.SendAdminEmailAfterProducerRegisters(id, input.ProducerName, input.Email, input.FirstName + " " + input.LastName);
            }

            return true;
        }

        public async Task<UserBasicInfoDto> RegisterWithFBAsync(UserFBRegisterInput input)
        {
            var id = await _userService.CreateWithFBAsync(input);
            var user = await GetBasicInfoById(id);
            return user;
        }

        public async Task<bool> UpdateUserInfoAsync(string id, UserUpdateInfoInput input)
        {
            return await _userService.UpdateUserAsync(id, input);
        }

        public async Task<bool> UpdateProducerInfoAsync(string id, ProducerUpdateInfoInput input)
        {
            return await _userService.UpdateProducerAsync(id, input);
        }

        public async Task<bool> UserBecomeProducerAsync(string id, UserBecomeProducerInput input)
        {
            // Send email to admin to let them know a user just became a producer 
            var emailToAdminSent = _emailSender.SendAdminEmailAfterUserUpdatedToProducer(id, input.ProducerName);

            return await _userService.BecomeProducerAsync(id, input);
        }

        public async Task<bool> ConfirmAccountAsync(string guid)
        {
            return await _userService.ConfirmAccountAsync(guid);
        }

        public async Task<UserBasicInfoDto> AuthenticateAsync(UserLoginInput input)
        {
            var entity = await _userService.AuthenticateAsync(input.Email, input.Password);
            if (entity == null)
            {
                throw new HttpStatusCodeException(401, new System.Collections.Generic.List<string> { "Adresa de email sau parola incorecte." });
            }
            if (!entity.IsActive)
            {
                throw new HttpStatusCodeException(401, new System.Collections.Generic.List<string> { "Contul tau nu a fost activat. Confirma contul pentru a te putea conecta." });
            }

            return await GetBasicInfoById(entity.Id.ToString());
        }

        #region Unicity Validators

        public async Task<bool> IsProducerNameUnique(string producerName, string id)
        {
            return await _userService.IsProducerNameUnique(producerName, id);
        }

        public async Task<bool> IsEmailUnique(string email, string id)
        {
            return await _userService.IsEmailUnique(email, id);
        }

        #endregion
    }
}
